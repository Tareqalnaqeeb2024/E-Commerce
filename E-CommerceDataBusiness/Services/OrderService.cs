using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataAccess.UnitOfWork;
using E_CommerceDataBusiness.Hubs;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services
{
  
    public class OrderService : IOrderService
    {
        //private readonly IOrderRepository _orderRepository;
        //private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly IEmailService _emailService;
        //private readonly IUserRepository  _userRepository;
        //private readonly IUserService  _userService;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IRedisService _redisCache;
        private readonly IUnitOfwork _unitOfwork;
        private const string KeyPrefix = "order:";

        public OrderService(IUnitOfwork unitOfwork, IMapper mapper , IProductRepository productRepository , IHubContext<ProductHub> hubContext ,
             IEmailService emailService , IUserService userService ,IRedisService redisService  ,IUserRepository userRepository , IHubContext<NotificationHub> notificationHubContext)
        {
            _unitOfwork = unitOfwork;
            _mapper = mapper;
            _hubContext = hubContext;
            _emailService = emailService;
            _redisCache = redisService;
            _notificationHubContext = notificationHubContext;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            string cachkey = $"{KeyPrefix}all";
            var cachorder = await _redisCache.GetAsync<List<OrderDTO>>(cachkey);

            if (cachorder != null)
            {
                return cachorder;
            }
            var orders = await _unitOfwork.orders.GetAllAsync();
            var result =  _mapper.Map<List<OrderDTO>>(orders);

            await _redisCache.SetAsync(cachkey, result,TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<IEnumerable<OrderDTO>> GetUserOrdersAsync(string userId)
        {
            var orders = await _unitOfwork.orders.GetByUserIdWithDetailsAsync(userId);
            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfwork.orders.GetByIdWithDetailsAsync(id);
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreate, string userId)
        {
            var order = _mapper.Map<Order>(orderCreate);
            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            order.Status = "Pending";

     

            foreach (var item in order.OrderItems)
            {
                var product =  await _unitOfwork.products.GetByIdAsync(item.ProductId);


                if (product == null || product.StockQuantity < item.Quantity)
                    throw new Exception("Quantity Not Exists");

                product.StockQuantity -= item.Quantity;
                _unitOfwork.products.Update(product);
             




                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", product.ProductId, product.StockQuantity);

            }

          await _unitOfwork.orders.AddAsync(order);
            _unitOfwork.Complete();

            var user = await _unitOfwork.users.GetByIdAsync(userId);

          //await   _emailService.SendEmailAsync(user.Email," // Created New Order //" ,$"Hello {user.UserName} Your Order with Id {createdOrder.OrderId} has Created with Panding Staust");
            await _notificationHubContext.Clients.All.SendAsync("ReceiveNewOrder", $"OderId : {order.OrderId} from user {user.UserName} ");
            await _notificationHubContext.Clients.Group("Admin").SendAsync("ReceiveNewOrder", $"OderId : {order.OrderId} from user {user.UserName} ");

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task UpdateOrderAsync(int id, OrderUpdateDTO updateDTO)
        {
            var order = await _unitOfwork.orders.GetByIdWithDetailsAsync(id);
            _mapper.Map(updateDTO, order);
            order.TotalAmount = order.OrderItems.Sum(o => o.Price * o.Quantity);
             _unitOfwork.orders.Update(order);
            _unitOfwork.Complete();

        }

        public async Task DeleteOrderAsync(int id)
        {
            var order  = await _unitOfwork.orders.GetByIdAsync(id);
             _unitOfwork.orders.Delete(order);
             _unitOfwork.Complete();   
        }

        public async Task CancelOrderAsync(int id, string userId, bool isAdmin)
        {
            var order = await _unitOfwork.orders.GetByIdAsync(id);
          
            if (!isAdmin && order.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this order.");

            if (order.Status != "Pending")
                throw new InvalidOperationException("Only pending orders can be canceled.");

            order.Status = "Canceled";
             _unitOfwork.orders.Update(order);
             _unitOfwork.Complete();
        }

        
        public async Task<PagedResult<OrderDTO>> GetOrdersPagedAsync(OrderPagination parameters, string? userId = null)
        {
            //string cacheKey = $"{KeyPrefix}paged:{userId}:{JsonConvert.SerializeObject(parameters)}";
            //var cachedResult = await _redisCache.GetAsync<PagedResult<OrderDTO>>(cacheKey);

            //if (cachedResult != null)
            //{
            //    return cachedResult;
            //}

            var pagedResult = await _unitOfwork.orders.GetPagedOrdersAsync(parameters, userId);
            var orderDtos = _mapper.Map<List<OrderDTO>>(pagedResult.Items);

            var result = new PagedResult<OrderDTO>
            {
                Items = orderDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

            //await _redisCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

       

        public async Task<IEnumerable<OrderDTO>> SearchStatusKeyOrId(string key)
        {
            var orderdto = await _unitOfwork.orders.SearchByStatusOrIdAsync(key);
            return _mapper.Map<List<OrderDTO>>(orderdto);
        }
    }
}

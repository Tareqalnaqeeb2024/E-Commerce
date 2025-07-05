using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Hubs;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services
{
  
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<ProductHub> _hubContext;
        private readonly IEmailService _emailService;
        private readonly IUserRepository  _userRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IRedisService _redisCache;
        private const string KeyPrefix = "order:";

        public OrderService(IOrderRepository orderRepository, IMapper mapper , IProductRepository productRepository , IHubContext<ProductHub> hubContext ,
             IEmailService emailService ,IRedisService redisService  ,IUserRepository userRepository , IHubContext<NotificationHub> notificationHubContext)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _hubContext = hubContext;
            _emailService = emailService;
            _redisCache = redisService;
            _userRepository = userRepository;
            _notificationHubContext = notificationHubContext;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            string cachkey = $"{KeyPrefix}all";
            var cachorder = await _redisCache.GetAsync<List<OrderDTO>>(cachkey);

            if(cachorder != null)
            {
                return cachorder;
            }
            var orders = await _orderRepository.GetAllWithDetailsAsync();
            var result =  _mapper.Map<List<OrderDTO>>(orders);

            await _redisCache.SetAsync(cachkey, result,TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task<IEnumerable<OrderDTO>> GetUserOrdersAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdWithDetailsAsync(userId);
            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
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
                var product =  await _productRepository.GetByIdAsync(item.ProductId);


                if (product == null || product.StockQuantity < item.Quantity)
                    throw new Exception("Quantity Not Exists");

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);


                await _hubContext.Clients.All.SendAsync("ReceiveStockUpdate", product.ProductId, product.StockQuantity);

            }

            var createdOrder = await _orderRepository.AddAsync(order);

            UserDTO user = await _userRepository.GetUserByIdAsync(userId);

          await   _emailService.SendEmailAsync(user.Email," // Created New Order //" ,$"Hello {user.UserName} Your Order with Id {createdOrder.OrderId} has Created with Panding Staust");
            await _notificationHubContext.Clients.All.SendAsync("ReceiveNewOrder", $"OderId : {createdOrder.OrderId} from user {createdOrder.User.UserName} ");
            await _notificationHubContext.Clients.Group("Admin").SendAsync("ReceiveNewOrder", $"OderId : {createdOrder.OrderId} from user {createdOrder.User.UserName} ");

            return _mapper.Map<OrderDTO>(createdOrder);
        }

        public async Task UpdateOrderAsync(int id, OrderUpdateDTO updateDTO)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            _mapper.Map(updateDTO, order);
            order.TotalAmount = order.OrderItems.Sum(o => o.Price * o.Quantity);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        public async Task CancelOrderAsync(int id, string userId, bool isAdmin)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (!isAdmin && order.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to cancel this order.");

            if (order.Status != "Pending")
                throw new InvalidOperationException("Only pending orders can be canceled.");

            order.Status = "Canceled";
            await _orderRepository.UpdateAsync(order);
        }
    }
}

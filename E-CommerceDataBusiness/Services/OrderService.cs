using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services
{
    // E_Commerce.Business/Services/OrderService.cs
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllWithDetailsAsync();
            return _mapper.Map<List<OrderDTO>>(orders);
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
            order.TotalAmount = orderCreate.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            var createdOrder = await _orderRepository.AddAsync(order);
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

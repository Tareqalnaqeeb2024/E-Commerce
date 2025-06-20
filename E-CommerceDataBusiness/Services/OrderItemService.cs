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
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public OrderItemService(
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
        {
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<OrderItemDTO> GetOrderItemByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be positive");

            var orderItem = await _orderItemRepository.GetByIdWithProductAsync(id);
            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found");

            return _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task<IEnumerable<OrderItemDTO>> GetAllOrderItemsAsync()
        {
            var orderItems = await _orderItemRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderItemDTO>>(orderItems);
        }

        public async Task<OrderItemDTO> CreateOrderItemAsync(OrderItemCreateDTO orderItemCreate)
        {
            var orderItem = _mapper.Map<OrderItem>(orderItemCreate);
            var createdItem = await _orderItemRepository.AddAsync(orderItem);
            return _mapper.Map<OrderItemDTO>(createdItem);
        }

        public async Task UpdateOrderItemAsync(int id, OrderItemUpdateDTO orderItemUpdate)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be positive");

            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found");

            _mapper.Map(orderItemUpdate, orderItem);
            await _orderItemRepository.UpdateAsync(orderItem);
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be positive");

            if (!await _orderItemRepository.ExistsAsync(id))
                throw new KeyNotFoundException("Order item not found");

            await _orderItemRepository.DeleteAsync(id);
        }
    }
}

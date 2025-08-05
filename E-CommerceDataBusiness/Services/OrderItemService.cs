using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataAccess.UnitOfWork;
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
       
        private readonly IMapper _mapper;
        private readonly IUnitOfwork _unitOfwork;

        public OrderItemService(
           
            IMapper mapper,
            IUnitOfwork unitOfwork)
        {
            _mapper = mapper;
            _unitOfwork = unitOfwork;
        }

        public async Task<OrderItemDTO> GetOrderItemByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be positive");

            var orderItem = await _unitOfwork.orderItems.GetByIdWithProductAsync(id);
            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found");

            return _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task<IEnumerable<OrderItemDTO>> GetAllOrderItemsAsync()
        {
            var orderItems = await _unitOfwork.orderItems.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderItemDTO>>(orderItems);
        }

        public async Task<OrderItemDTO> CreateOrderItemAsync(OrderItemCreateDTO orderItemCreate)
        {
            var orderItem = _mapper.Map<OrderItem>(orderItemCreate);
             await _unitOfwork.orderItems.AddAsync(orderItem);
                   _unitOfwork.Complete();
            return _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task UpdateOrderItemAsync(int id, OrderItemUpdateDTO orderItemUpdate)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be positive");

            var orderItem = await _unitOfwork.orderItems.GetByIdAsync(id);
            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found");

            _mapper.Map(orderItemUpdate, orderItem);
            _unitOfwork.orderItems.Update(orderItem);
            await _unitOfwork.CompleteAsync();
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be positive");

            if (!await _unitOfwork.orderItems.ExistsAsync(id))
                throw new KeyNotFoundException("Order item not found");
            var orderitem = await _unitOfwork.orderItems.GetByIdAsync(id);
            
            _unitOfwork.orderItems.Delete(orderitem);
            await _unitOfwork.CompleteAsync();
        }
    }
}

using E_CommerceDataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface IOrderItemService
    {
        Task<OrderItemDTO> GetOrderItemByIdAsync(int id);
        Task<IEnumerable<OrderItemDTO>> GetAllOrderItemsAsync();
        Task<OrderItemDTO> CreateOrderItemAsync(OrderItemCreateDTO orderItemCreate);
        Task UpdateOrderItemAsync(int id, OrderItemUpdateDTO orderItemUpdate);
        Task DeleteOrderItemAsync(int id);
    }
}

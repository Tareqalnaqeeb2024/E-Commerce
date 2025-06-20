using E_CommerceDataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDTO>> GetUserOrdersAsync(string userId);
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreate, string userId);
        Task UpdateOrderAsync(int id, OrderUpdateDTO updateDTO);
        Task DeleteOrderAsync(int id);
        Task CancelOrderAsync(int id, string userId, bool isAdmin);
    }
}

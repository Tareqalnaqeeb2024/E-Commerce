using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> GetByIdAsync(int id);
        Task<OrderItem> GetByIdWithProductAsync(int id);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem> AddAsync(OrderItem orderItem);
        Task UpdateAsync(OrderItem orderItem);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

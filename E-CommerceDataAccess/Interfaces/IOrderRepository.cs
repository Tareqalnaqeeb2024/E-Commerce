using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task<IEnumerable<Order>> GetByUserIdWithDetailsAsync(string userId);
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}

using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        Task<OrderItem> GetByIdWithProductAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

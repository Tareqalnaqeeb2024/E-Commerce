using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Order> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetByUserIdWithDetailsAsync(string userId);
        Task<PagedResult<Order>> GetPagedOrdersAsync(OrderPagination parameters, string? userId = null);
        Task<IEnumerable<Order>> SearchByStatusOrIdAsync(string keyword );

    }
}

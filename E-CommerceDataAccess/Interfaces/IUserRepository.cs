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
    public interface IUserRepository
    {
        Task<UserAccount> GetByIdAsync(string id);
        Task<UserAccount> GetByUsernameAsync(string username);
        Task<IEnumerable<UserAccount>> GetAllAsync();
        Task<bool> CreateAsync(UserAccount user, string password, string role);
        Task<bool> UpdateAsync(UserAccount user);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string username);

        // Extended functionality
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> UpdateUserRolesAsync(string userId, IEnumerable<string> roles);

        // Dashboard
        Task<DashboardStatsDto> GetDashboardStatsAsync();

        // Pagination
        Task<PagedResult<UserAccount>> GetPagedUsersAsync(UserPaginationParams parameters);
    }
}


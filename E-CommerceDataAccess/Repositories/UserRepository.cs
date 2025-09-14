using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Repositories
{
    
    public class UserRepository :BaseRepository<UserAccount> , IUserRepository
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(
            UserManager<UserAccount> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context) :base (context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserAccount> GetByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<UserAccount> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

      

        public async Task<bool> CreateAsync(UserAccount user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded && !string.IsNullOrEmpty(role))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
            }

            return result.Succeeded;
        }

        public async Task<bool> UpdateAsync(UserAccount user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Enumerable.Empty<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove old roles
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new roles
            var result = await _userManager.AddToRolesAsync(user, roles);

            return result.Succeeded;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _userManager.Users.CountAsync();

            var recentOrders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            return new DashboardStatsDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                RecentOrders = recentOrders.Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                 
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                }).ToList()
            };
        }

        public async Task<PagedResult<UserAccount>> GetPagedUsersAsync(UserPaginationParams parameters)
        {
            var query = _userManager.Users.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
            {
                query = query.Where(u =>
                    u.UserName.Contains(parameters.SearchTerm) ||
                    u.Email.Contains(parameters.SearchTerm));
            }

            // Apply role filter
            if (!string.IsNullOrEmpty(parameters.RoleFilter))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(parameters.RoleFilter);
                var userIds = usersInRole.Select(u => u.Id);
                query = query.Where(u => userIds.Contains(u.Id));
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "email" => parameters.SortDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "username" => parameters.SortDescending
                    
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<UserAccount>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
    }
}
using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Repositories
{
    
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {

        public OrderRepository(AppDbContext context) :base(context)
        {
        }

      
        public async Task<Order> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }


        public async Task<IEnumerable<Order>> GetByUserIdWithDetailsAsync(string userId)
        {

            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        


        public async Task<PagedResult<Order>> GetPagedOrdersAsync(OrderPagination parameters, string? userId = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // Apply user filter if specified
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(parameters.Status))
            {
                query = query.Where(o => o.Status == parameters.Status);
            }

            // Apply date range filter
            if (parameters.StartDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= parameters.StartDate);
            }
            if (parameters.EndDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= parameters.EndDate);
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "amount" => parameters.SortDescending
                    ? query.OrderByDescending(o => o.TotalAmount)
                    : query.OrderBy(o => o.TotalAmount),
                "status" => parameters.SortDescending
                    ? query.OrderByDescending(o => o.Status)
                    : query.OrderBy(o => o.Status),
                _ => parameters.SortDescending  // Default: sort by date
                    ? query.OrderByDescending(o => o.OrderDate)
                    : query.OrderBy(o => o.OrderDate)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Order>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        public async Task<IEnumerable<Order>> SearchByStatusOrIdAsync(string keyword )
        {
            return await _context.Orders
                 .Where(o =>  o.Status.Contains(keyword) )
                 .ToListAsync();
        }
    }
}

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
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) :base(context)
        {
            _context = context;
        }

        //public async Task<Order> GetByIdAsync(int id)
        //{
        //    return await _context.Orders.FindAsync(id);
        //}

        public async Task<Order> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        //public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
        //{
        //    return await _context.Orders
        //        .Include(u => u.User)
        //        .Include(o => o.OrderItems)
        //        .ThenInclude(o => o.Product)
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<Order>> GetByUserIdWithDetailsAsync(string userId)
        {

            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        //public async Task<Order> AddAsync(Order order)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();


        //    try
        //    {
        //        foreach (var item in order.OrderItems)
        //        {
        //            var product = await _context.Products.FindAsync(item.ProductId);
        //            if (product == null || product.StockQuantity < item.Quantity)
        //                throw new Exception("Insufficient stock.");
        //            product.StockQuantity -= item.Quantity;
        //            _context.Products.Update(product);

        //        }
        //        _context.Orders.Add(order);
        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        return order;


        //    }
        //    catch (Exception)
        //    {

        //        await transaction.RollbackAsync();
        //        throw;
        //    }


        //}

        //public async Task UpdateAsync(Order order)
        //{
        //    _context.Orders.Update(order);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteAsync(int id)
        //{
        //    var order = await GetByIdAsync(id);
        //    if (order != null)
        //    {
        //        _context.Orders.Remove(order);
        //        await _context.SaveChangesAsync();
        //    }
        //}


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
    }
}

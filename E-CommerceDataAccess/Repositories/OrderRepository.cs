using E_CommerceDataAccess.Data;
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
    // E_Commerce.DataAccess/Repositories/OrderRepository.cs
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetAllWithDetailsAsync()
        {
            return await _context.Orders
                .Include(u => u.User)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByUserIdWithDetailsAsync(string userId)
        {

            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        public async Task<Order> AddAsync(Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();


            try
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || product.StockQuantity < item.Quantity)
                        throw new Exception("Insufficient stock.");
                    product.StockQuantity -= item.Quantity;
                    _context.Products.Update(product);

                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;


            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                throw;
            }

            
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await GetByIdAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}

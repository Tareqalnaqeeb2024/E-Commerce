// E_Commerce.DataAccess/Repositories/OrderItemRepository.cs

using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> GetByIdAsync(int id)
        {
            return await _context.Items.FindAsync(id);
        }

        public async Task<OrderItem> GetByIdWithProductAsync(int id)
        {
            return await _context.Items
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<OrderItem> AddAsync(OrderItem orderItem)
        {
            _context.Items.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var orderItem = await GetByIdAsync(id);
            if (orderItem != null)
            {
                _context.Items.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Items.AnyAsync(e => e.OrderItemId == id);
        }
    }
}
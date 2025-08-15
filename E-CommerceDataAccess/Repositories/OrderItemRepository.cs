
using E_CommerceDataAccess.BaseRepositry;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {

        public OrderItemRepository(AppDbContext context):base(context)
        {
        }

        public async Task<OrderItem> GetByIdWithProductAsync(int id)
        {
            return await _context.Items
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Items.AnyAsync(e => e.OrderItemId == id);
        }
    }
}
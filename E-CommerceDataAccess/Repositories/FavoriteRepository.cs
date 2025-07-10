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
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext  _context;
        public FavoriteRepository( AppDbContext Context)
        {
            _context = Context;
        }

        public async Task AddToFavorite(string userId, int productId)
        {
            // التحقق من عدم وجود المنتج بالفعل في المفضلة

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null)
            {
                throw new KeyNotFoundException("Product Not Found");
            }
            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
           if (existingFavorite != null)
            {
                throw new InvalidOperationException("المنتج موجود بالفعل في المفضلة");
            }

            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };

           await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();


        }

        public async Task<IEnumerable<Favorite>> GetAllFavorites(string userId)
        {
           return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Product)
                .ToListAsync(); 
        }

        public async Task RemoveFromFavorite(string userId, int productId)
        {
            var favorite = await _context.Favorites
                 .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
            if (favorite == null)
            {
                throw new KeyNotFoundException("لم يتم العثور على المنتج في المفضلة");
            }
             _context.Favorites.Remove(favorite);
             await _context.SaveChangesAsync(); 
        }
    }
}

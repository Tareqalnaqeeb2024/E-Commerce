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
    public class ProductRepository :BaseRepository<Product>, IProductRepository
    {

        public ProductRepository(AppDbContext context) :base (context) 
        {
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

    

        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        

        public async Task<bool> AnyByCategoryIdAsync(int categoryId)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Product>> GetAllWithCategoryNameAsync( string categoryname)
        {
            return await _context.Products.Include(c => c.Category).Where(n => n.Category.Name  == categoryname).ToListAsync();
        }
        public async Task<IEnumerable<Product>> SearchByNameOrDescriptionAsync(string keyword)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p =>
                    p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword))
                .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity >= 1)
                .ToListAsync();
        }

     
        public async Task<PagedResult<Product>> GetPagedProductsAsync(ProductPagination parameters)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(parameters.CategoryName))
            {
                query = query.Where(p => p.Category.Name == parameters.CategoryName);
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= parameters.MinPrice);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= parameters.MaxPrice);
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "name" => parameters.SortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "price" => parameters.SortDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                _ => query.OrderBy(p => p.Name) // Default sorting
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

    }
}

using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByIdWithCategoryAsync(int id);
        Task<Product> AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> AnyByCategoryIdAsync(int categoryId);
    }
}

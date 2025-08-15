using E_CommerceDataAccess.BaseRepositry;
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
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllWithCategoryAsync();
        Task<Product> GetByIdWithCategoryAsync(int id);
        Task<bool> AnyByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetAllWithCategoryNameAsync(string categoryname);
        Task<IEnumerable<Product>> SearchByNameOrDescriptionAsync(string keyword);
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<PagedResult<Product>> GetPagedProductsAsync(ProductPagination parameters);
    }
}

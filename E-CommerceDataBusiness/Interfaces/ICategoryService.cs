using E_CommerceDataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO createDTO);
        Task UpdateCategoryAsync(int id, CategoryUpdateDTO updateDTO);
        Task DeleteCategoryAsync(int id);
    }
}

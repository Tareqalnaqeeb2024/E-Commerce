using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services
{
    // E_Commerce.Business/Services/CategoryService.cs
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO createDTO)
        {
            var category = _mapper.Map<Category>(createDTO);
            var createdCategory = await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryDTO>(createdCategory);
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDTO updateDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            _mapper.Map(updateDTO, category);
            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            bool hasProducts = await _productRepository.AnyByCategoryIdAsync(id);
            if (hasProducts) throw new InvalidOperationException("Cannot delete Category with Related Products");

            await _categoryRepository.DeleteAsync(id);
        }
    }
}

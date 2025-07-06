using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services
{
   
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisCache;
        private const string CacheKeyPrefix = "category:";

        public CategoryService(
            ICategoryRepository categoryRepository,
            IProductRepository productRepository,
            IMapper mapper,
            IRedisService redisService)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _redisCache = redisService;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            string cacheKey = $"{CacheKeyPrefix}all";
            var cachedCategories = await _redisCache.GetAsync<IEnumerable<CategoryDTO>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }
            var categoriesDto = await _categoryRepository.GetAllAsync();
            await _redisCache.SetAsync(cacheKey, categoriesDto, TimeSpan.FromMinutes(30));

            return _mapper.Map<IEnumerable<CategoryDTO>>(categoriesDto);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            string cacheKey = $"{CacheKeyPrefix}{id}";

            // Try cache first
            var cachedCategory = await _redisCache.GetAsync<CategoryDTO>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }
            var categoryDto = await _categoryRepository.GetByIdAsync(id);
            if (categoryDto == null) throw new KeyNotFoundException("Category not found");

            await _redisCache.SetAsync(cacheKey, categoryDto, TimeSpan.FromMinutes(30));

            return _mapper.Map<CategoryDTO>(categoryDto);
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

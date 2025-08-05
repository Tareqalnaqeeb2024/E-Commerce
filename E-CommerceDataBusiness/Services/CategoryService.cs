using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataAccess.UnitOfWork;
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
        private readonly IMapper _mapper;
        private readonly IRedisService _redisCache;
        private const string CacheKeyPrefix = "category:";
        private readonly IUnitOfwork _unitOfwork;

        public CategoryService(
            IMapper mapper,
            IRedisService redisService , IUnitOfwork unitOfwork)
        {
           
            _mapper = mapper;
            _redisCache = redisService;
            _unitOfwork = unitOfwork;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            string cacheKey = $"{CacheKeyPrefix}all";
            var cachedCategories = await _redisCache.GetAsync<IEnumerable<CategoryDTO>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }
            var categoriesDto = await _unitOfwork.categories.GetAllAsync();

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
            
            var categoryDto = await _unitOfwork.categories.GetByIdAsync(id);

            if (categoryDto == null) throw new KeyNotFoundException("Category not found");

            await _redisCache.SetAsync(cacheKey, categoryDto, TimeSpan.FromMinutes(30));

            return _mapper.Map<CategoryDTO>(categoryDto);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryCreateDTO createDTO)
        {
            var category = _mapper.Map<Category>(createDTO);
           await _unitOfwork.categories.AddAsync(category);
            await _unitOfwork.CompleteAsync();

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDTO updateDTO)
        {
            var category = await _unitOfwork.categories.GetByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            _mapper.Map(updateDTO, category);
           _unitOfwork.categories.Update(category);
            await _unitOfwork.CompleteAsync();

            await _redisCache.RemoveAsync($"{CacheKeyPrefix}{id}");

        }

        public async Task DeleteCategoryAsync(int id)
        {
            bool hasProducts = await _unitOfwork.products.AnyByCategoryIdAsync(id);
            if (hasProducts) throw new InvalidOperationException("Cannot delete Category with Related Products");
            var category = await _unitOfwork.categories.GetByIdAsync(id);
            _unitOfwork.categories.Delete(category);
            await _unitOfwork.CompleteAsync();
        }
    }
}

using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataBusiness.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace E_CommerceDataBusiness.Services
{
    public class FavoriteServices : IFavoriteServices
    {
        private readonly IFavoriteRepository  _favoriteRepository;
        private readonly IProductService _productService  ;
        private readonly IMapper _mapper;

        public FavoriteServices(IFavoriteRepository favoriteRepository, IProductService productRepository , IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _productService = productRepository;
            _mapper = mapper;
        }

        public async Task AddToFavorite(string userId, int productId)
        {
            //var product = _productService.GetProductByIdAsync(productId);
            //if (product == null)
            //{
            //    throw new KeyNotFoundException("المنتج غير موجود");
            //}

            await _favoriteRepository.AddToFavorite(userId, productId);
        }

       

        public async Task<IEnumerable<FavoriteDTO>> GetUserFavorites(string userId)
        {
            var favorites = await _favoriteRepository.GetAllFavorites(userId);

            return _mapper.Map<IEnumerable<FavoriteDTO>>(favorites);

        }

        public async Task RemoveFromFavorite(string userId, int productId)
        {
            await _favoriteRepository.RemoveFromFavorite(userId, productId);
        }
    }
}

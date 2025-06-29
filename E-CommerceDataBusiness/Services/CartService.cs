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
    public class CartService : ICartService
    {
        private readonly ICartSRepository _cartSRepository;

        public CartService( ICartSRepository cartSRepository)
        {
            _cartSRepository = cartSRepository;
        }

        public Task AddToCartAsync(CreateCartItemDTO cartItemDTO, string UserId)
        {
          return  _cartSRepository.AddToCartAsync(cartItemDTO, UserId);
        }

        public  async Task DecrementItemQuantityAsnyc(string UserId, int ProductId)
        {
         await    _cartSRepository.DecrementItemQuantityAsnyc(UserId, ProductId);
        }

        public  Task<CartDTO> GetCartAsync(string UserId)
        {
             return _cartSRepository.GetCartAsync(UserId);
        }

        public async Task IncrementItemQuantityAsnyc(int ProductId, string UserID)
        {
            await _cartSRepository.IncrementItemQuantityAsnyc(ProductId, UserID);
        }

        public Task RemoveFromCartAsync(string UserId, int ProductId)
        {
           return _cartSRepository.RemoveFromCartAsync(UserId, ProductId);
        }
    }
}

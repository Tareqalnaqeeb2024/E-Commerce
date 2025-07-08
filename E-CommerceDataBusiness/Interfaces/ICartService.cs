using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface ICartService 
    {

        Task<CartDTO> GetCartAsync(string UserId);
        Task AddToCartAsync(CreateCartItemDTO cartItemDTO, string UserId);
        Task RemoveFromCartAsync(string UserId, int ProductId);
        Task IncrementItemQuantityAsnyc(int ProductId, string UserID);
        Task DecrementItemQuantityAsnyc(string UserId, int ProductId);

    }
}

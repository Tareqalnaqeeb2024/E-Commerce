using E_CommerceDataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
  public   interface IFavoriteServices
    {
        Task AddToFavorite(string userId, int productId);
        Task RemoveFromFavorite(string userId, int productId);
        Task<IEnumerable<FavoriteDTO>> GetUserFavorites(string userId);
      
    }
}

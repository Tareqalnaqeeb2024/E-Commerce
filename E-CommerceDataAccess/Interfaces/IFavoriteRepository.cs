using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IFavoriteRepository
    {
        Task AddToFavorite(string userId, int productId);
        Task RemoveFromFavorite(string userId, int productId);
        Task<IEnumerable<Favorite>> GetAllFavorites(string userId);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
    public class FavoriteDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public string CategoryName { get; set; }

    }
    public class IsFavorite
    {
        public int ProductId { set; get; }
        public bool Is_Favorite { set; get; }
    }
}
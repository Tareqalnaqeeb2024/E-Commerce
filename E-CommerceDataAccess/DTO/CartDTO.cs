using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
    public class CartDTO
    {
        public string UserID { get; set; }
        public List<GetCartItemDTO> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

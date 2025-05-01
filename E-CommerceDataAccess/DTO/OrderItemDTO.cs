using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderItemCreateDTO
    {
        public int ProductId { get; set; }
        public int Quentity { get; set; }
        public decimal Price { get; set; }

    }
    public class OrderItemUpdateDTO
    {
        public int ProductId { get; set; }
        public int Quentity { get; set; }
        public decimal Price { get; set; }

    }
}

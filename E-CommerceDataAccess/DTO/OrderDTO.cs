using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }

    public class OrderCreateDTO
    {
        public string UserId { get; set; }
        public List<OrderItemCreateDTO> OrderItems { get; set; } = new List<OrderItemCreateDTO>();
    }
}

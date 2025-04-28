using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int Quantity { get; set; }
      
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(ProductId))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public Order Order { get; set; }


    }
}

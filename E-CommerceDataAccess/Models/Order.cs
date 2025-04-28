using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "pending";

        public ICollection<OrderItem> OrderItems { get; set; }

        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }  // Must match IdentityUser's key type
        public UserAccount User { get; set; }

    }
}

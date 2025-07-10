using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public List<CartItem> Items { get; set;} = new List<CartItem>();

        [ForeignKey(nameof(UserAccount))]
        public string UserID { get; set; }
        public UserAccount UserAccount { get; set; }
        [NotMapped]
        public decimal TotalAmount => Items.Sum(item => item.Product.Price * item.Quantity);

       
    }
}

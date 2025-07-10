using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Models
{
    public class Favorite
    {
       
       
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }  // Must match IdentityUser's key type
        public UserAccount User { get; set; }

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}

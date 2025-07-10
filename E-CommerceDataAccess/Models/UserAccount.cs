using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Models
{
    public class UserAccount : IdentityUser
    {
        public ICollection<Order> Orders { get; set; }
        public ICollection<Favorite> Favorites { get; set; } 
  
       
    }
}

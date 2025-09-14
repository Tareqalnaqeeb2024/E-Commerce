using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
    public class UserDTO
    {
        public string userId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string  Roles { get; set; } 
    }
    public class RegisterDTO
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }
       
    }
    public class CreateNewUserDTO
    {

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Roles { get; set; }

    }


}

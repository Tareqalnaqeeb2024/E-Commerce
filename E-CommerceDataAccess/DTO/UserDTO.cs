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
    public class LoginDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ForgetPasswordDto
    {
        public string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserDTO User { get; set; }
    }
    public class TokenDTO
    {
      

        public string Email { get; set; }
        
        public string UserID { get; set; }
        public string Token { get; set; }
    }

    public class VerfiyCodeDto
    {
        public string Email { get; set; }
        public string CodeOTP { get; set; }
    }


}

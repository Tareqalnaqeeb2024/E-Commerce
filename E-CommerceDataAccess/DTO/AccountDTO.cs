using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.DTO
{
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

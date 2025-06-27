using E_CommerceDataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
   public interface IAccountService
    {
        Task<TokenDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<TokenDTO> LoginAsync(LoginDTO loginDTO);
        Task<string> ForgotPasswordAsync (ForgetPasswordDto forgetPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> VerifyOTPAsync(VerfiyCodeDto verfiyCodeDto);
    }
}

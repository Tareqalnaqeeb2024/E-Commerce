using E_Commerce.Basic;
using E_CommerceDataAccess.DTO;
using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("reigster")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDTO)
        {

            try
            {
                var reslut = await _accountService.RegisterAsync(registerDTO);
                return Ok(reslut);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var result = await _accountService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return Unauthorized(ex.Message);
            }

        }
        [HttpPut("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            var result = await _accountService.ResetPasswordAsync(resetPassword);
            return result ? Ok(result) : BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult<BaseResponse<string>>> ForgetPassword([FromBody] ForgetPasswordDto request)
        {

            var result = await _accountService.ForgotPasswordAsync(request);
            return result != null ? Ok(result) : BadRequest(result);
        }

        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<BaseResponse<bool>>> VerifyOTP([FromBody] VerfiyCodeDto verify)
        {

            var result = await _accountService.VerifyOTPAsync(verify);
            return result ? Ok(result) : BadRequest(result);
        }



    }
}

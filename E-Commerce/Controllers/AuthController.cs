using E_Commerce.Basic;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Services.ExternalServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            try
            {
                var result = await _accountService.ResetPasswordAsync(resetPassword);
                return Ok(result);
            }

            
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

                
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto request)
        {

            var result = await _accountService.ForgotPasswordAsync(request);
            return result != null ? Ok(result) : NotFound(result);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerfiyCodeDto verify)
        {

            var result = await _accountService.VerifyOTPAsync(verify);
            return result ? Ok(result) : BadRequest(result);
        }
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest("Google authentication failed");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var givenName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var surname = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim not received from Google");

            var token = await _accountService.HandleGoogleUserAsync(email, givenName, surname);

            return Ok(token);
        }



    }
}

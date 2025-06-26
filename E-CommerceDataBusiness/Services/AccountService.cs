using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceDataBusiness.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserAccount> _userManager;

        public AccountService(AppDbContext context, UserManager<UserAccount> userManager , ITokenService tokenService, IRedisService redisService, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _redisService = redisService;
            _emailService = emailService;
        }
        public  async Task<string> ForgotPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            var user = await _userManager.Users
         .Where(u => u.Email == forgetPasswordDto.Email)
         .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("Email not found");

            var otp = new Random().Next(100000, 999999).ToString();
            await _redisService.SetOtpAsync(forgetPasswordDto.Email, otp, TimeSpan.FromMinutes(10));
            await _emailService.SendEmailAsync(forgetPasswordDto.Email, "Reset OTP", $"Your OTP is: {otp}");
            return "OTP sent successfully";
        }

        public async Task<TokenDTO> LoginAsync(LoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                throw new Exception("tarew");
            }
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (user == null )
                throw new Exception("Invalid credentials UserName or Password");

            var reslut = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!reslut)
                throw new Exception("Invalid credentials");
           
            var token = new TokenDTO
            {
                Token = await _tokenService.GenerateTokenAsync(user),
                UserId = user.Id,
                Email = user.Email,
                Name = user.UserName,
            };
            var decodedToken = Uri.UnescapeDataString(token.Token);
            Console.WriteLine(Uri.UnescapeDataString(decodedToken));

            return token;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmNewPassword)
                throw new Exception("Passwords do not match");

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                throw new Exception("User not found");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);
            Console.WriteLine(token);
            if (!result.Succeeded)
            {
                // يمكنك معرفة السبب:
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }

                return false;
            }
         


                return true;
        }

        public async Task<bool> VerifyOTPAsync(VerfiyCodeDto verifyCodeDto)
        {
            // 1. التحقق من وجود المستخدم
            var user = await _userManager.Users
                    .Where(u => u.Email == verifyCodeDto.Email)
                    .FirstOrDefaultAsync();

            if (user == null)
                throw new ArgumentException("User not found", nameof(verifyCodeDto.Email));

            // 2. استرجاع كود OTP من Redis (باستخدام await لأنها عملية غير متزامنة)
            var storedOtp = await _redisService.GetOtpAsync(verifyCodeDto.Email);

            if (string.IsNullOrEmpty(storedOtp))
                throw new ArgumentException("OTP expired or not found", nameof(verifyCodeDto.Email));

            // 3. مقارنة الكود المدخل بالكود المخزن
            if (storedOtp != verifyCodeDto.CodeOTP)
                throw new ArgumentException("Invalid OTP", nameof(verifyCodeDto.CodeOTP));

            // 4. إذا وصلنا إلى هنا، الكود صحيح
            return true;
        }
    }
}

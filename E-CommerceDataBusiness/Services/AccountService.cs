using AutoMapper;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_CommerceDataBusiness.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly IEmailService _emailService;
        private readonly UserManager<UserAccount> _userManager;
        private readonly IMapper _mapper;

        public AccountService(AppDbContext context, UserManager<UserAccount> userManager , ITokenService tokenService, IRedisService redisService,
            IEmailService emailService , IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _redisService = redisService;
            _emailService = emailService;
            _mapper = mapper;
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
                Email = user.Email,
                UserID = user.Id              
            };
            var decodedToken = Uri.UnescapeDataString(token.Token);
            Console.WriteLine(Uri.UnescapeDataString(decodedToken));

            return token;
        }

        public async Task<TokenDTO> RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDTO.UserName);
            if (existingUser != null)
                throw new Exception("A user with this UserName already exists.");

            if (registerDTO.Password != registerDTO.ConfirmPassword)
                throw new Exception("Password and Confirm Password do not match");

            if (!new EmailAddressAttribute().IsValid(registerDTO.Email))
                throw new Exception("Invalid email format.");

            var user = new UserAccount
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
               
            };


            var result = await _userManager.CreateAsync(user, registerDTO.Password);


            await _userManager.AddToRoleAsync(user, "User");

            if (!result.Succeeded)
                throw new Exception("User creation failed");
            else
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "مرحبًا بك في تطبيقنا!",
                    $"مرحبًا {user.UserName} 👋\n\nنحن سعداء بانضمامك إلى منصتنا."
                );
            }

            return new TokenDTO
            {
                Email = user.Email,
                Token = await _tokenService.GenerateTokenAsync(user),
                UserID = user.Id
                
                
            };
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

    
            var storedOtp = await _redisService.GetOtpAsync(verifyCodeDto.Email);

            if (string.IsNullOrEmpty(storedOtp))
                throw new ArgumentException("OTP expired or not found", nameof(verifyCodeDto.Email));

         
            if (storedOtp != verifyCodeDto.CodeOTP)
                throw new ArgumentException("Invalid OTP", nameof(verifyCodeDto.CodeOTP));

            
            return true;
        }
    }
}

//using AutoMapper;
//using E_Commerce.Basic;
//using E_CommerceDataAccess.DTO;
//using E_CommerceDataAccess.Interfaces;
//using E_CommerceDataAccess.Models;
//using E_CommerceDataBusiness.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace E_CommerceDataBusiness.Services
//{
//    public class AuthService : IAuthService
//    {
//        private readonly UserManager<UserAccount> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly JwtSettings _jwtSettings;
//        private readonly IOrderRepository _orderRepository;
//        private readonly IProductRepository _productRepository;
//        private readonly IMapper _mapper;

//        public AuthService(
//            UserManager<UserAccount> userManager,
//            RoleManager<IdentityRole> roleManager,
//            IOptions<JwtSettings> jwtSettings,
//            IOrderRepository orderRepository,
//            IProductRepository productRepository,
//            IMapper mapper)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _jwtSettings = jwtSettings.Value;
//            _orderRepository = orderRepository;
//            _productRepository = productRepository;
//            _mapper = mapper;
//        }

//        public async Task<AuthResponse> LoginAsync(LoginDTO loginDTO)
//        {
//            var user = await _userManager.FindByNameAsync(loginDTO.UserName);
//            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
//                throw new UnauthorizedAccessException("Invalid credentials");

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, user.UserName),
//                new Claim(ClaimTypes.NameIdentifier, user.Id),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };

//            var roles = await _userManager.GetRolesAsync(user);
//            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

//            var token = GenerateJwtToken(claims);
//            return new AuthResponse
//            {
//                Token = new JwtSecurityTokenHandler().WriteToken(token),
//                Expiration = token.ValidTo,
//                UserId = user.Id
//            };
//        }

//        public async Task<string> RegisterAsync(UserDTO userDTO, string role = "User")
//        {
//            var user = new UserAccount
//            {
//                UserName = userDTO.UserName,
//                Email = userDTO.Email
//            };

//            var result = await _userManager.CreateAsync(user, userDTO.Password);
//            if (!result.Succeeded)
//                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

//            if (!await _roleManager.RoleExistsAsync(role))
//                await _roleManager.CreateAsync(new IdentityRole(role));

//            await _userManager.AddToRoleAsync(user, role);
//            return $"User {user.UserName} registered successfully";
//        }

//        public async Task<string> RegisterAdminAsync(UserDTO userDTO)
//        {
//            return await RegisterAsync(userDTO, "Admin");
//        }

//        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
//        {
//            var users = _userManager.Users.ToList();
//            var userDtos = new List<UserDTO>();

//            foreach (var user in users)
//            {
//                var roles = await _userManager.GetRolesAsync(user);
//                userDtos.Add(new UserDTO
//                {
//                    userId = user.Id,
//                    UserName = user.UserName,
//                    Email = user.Email,
//                    Phone = user.PhoneNumber,
//                    Roles = roles.ToList()
//                });
//            }

//            return userDtos;
//        }

//        public async Task<string> DeleteUserAsync(string userId)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//                throw new KeyNotFoundException("User not found");

//            var result = await _userManager.DeleteAsync(user);
//            if (!result.Succeeded)
//                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

//            return $"User {user.UserName} deleted successfully";
//        }

//        public async Task<string> UpdateUserAsync(string userId, UserDTO userDTO)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//                throw new KeyNotFoundException("User not found");

//            user.UserName = userDTO.UserName;
//            user.Email = userDTO.Email;
//            user.PhoneNumber = userDTO.Phone;

//            var result = await _userManager.UpdateAsync(user);
//            if (!result.Succeeded)
//                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

//            if (userDTO.Roles != null && userDTO.Roles.Any())
//            {
//                var currentRoles = await _userManager.GetRolesAsync(user);
//                var rolesToAdd = userDTO.Roles.Except(currentRoles);
//                var rolesToRemove = currentRoles.Except(userDTO.Roles);

//                await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
//                await _userManager.AddToRolesAsync(user, rolesToAdd);
//            }

//            return $"User {user.UserName} updated successfully";
//        }

//csdf

//        private JwtSecurityToken GenerateJwtToken(List<Claim> claims)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            return new JwtSecurityToken(
//                issuer: _jwtSettings.Issuer,
//                audience: _jwtSettings.Audience,
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(1),
//                signingCredentials: creds);
//        }
//    }
//}

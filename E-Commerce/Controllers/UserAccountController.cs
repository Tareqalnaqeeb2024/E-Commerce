using Azure.Identity;
using E_Commerce.Basic;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        public readonly UserManager<UserAccount> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly JwtSettings _jwtSettings;
        public readonly AppDbContext _context;

        public UserAccountController( UserManager<UserAccount> userManager, IOptions<JwtSettings> jwtSettings ,
            RoleManager<IdentityRole> roleManager , AppDbContext context )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }
       
        [HttpPost("[action]")]

        public async Task<IActionResult> CreateNewUser( UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                UserAccount userAccount = new()
                {
                    UserName = userDTO.UserName,
                    Email = userDTO.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(userAccount, userDTO.Password);

                if (result.Succeeded)
                {
                    return Ok("Added User Successfuly");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                UserAccount? user = await  _userManager.FindByNameAsync(loginDTO.UserName);

                if(user != null)
                {
                    if (await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                    {
                        // First Get Claims

                        var Claims = new List<Claim>();

                        Claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            Claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                            
                        }

                        //second Get SigningCredentials

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                        var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        // third initial the token 

                        var token = new JwtSecurityToken(
                            claims: Claims,
                            issuer: _jwtSettings.Issuer,
                            audience: _jwtSettings.Audience,
                            expires: DateTime.UtcNow.AddHours(1),
                            signingCredentials: sc);

                        var _token = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Ok(_token);

                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "UserName is Invalid");
                }

            }
            return BadRequest(ModelState);
        }
      
        [HttpPost("CreateNewAdmin")]
         public async Task<ActionResult> CreateNewAdmin( UserDTO userDTO , string role = "Admin")
        {
            if (ModelState.IsValid)
            {
                UserAccount userAccount = new()
                {
                    UserName = userDTO.UserName,
                    Email = userDTO.Email,
                };

                IdentityResult result = await _userManager.CreateAsync(userAccount, userDTO.Password);

                if (result.Succeeded)
                {
                    if (! await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }

                    await _userManager.AddToRoleAsync(userAccount, role);
                    return Ok($"Added a dmin user  Successfuly  with role {role} ");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return BadRequest(ModelState);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDTO
                {
                    Phone = user.PhoneNumber,
                    Password = user.PasswordHash,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User deleted successfully");
        }
   
    [Authorize(Roles = "Admin")]
    [HttpGet("DashboardStats")]
    public async Task<ActionResult> GetDashboardStats()
    {
        var totalOrders = await   _context.Orders.CountAsync();
        var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
        var totalProducts = await _context.Products.CountAsync();
        var totalUsers = await _context.Users.CountAsync();

            var dashboardStats = new DashboardStatsDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers
              
            };
            return Ok(dashboardStats);
        }
  }
}

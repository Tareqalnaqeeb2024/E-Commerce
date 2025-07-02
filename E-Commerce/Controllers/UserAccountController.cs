using E_Commerce.Business.Services;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserAccountController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AppDbContext _context;

        public UserAccountController(UserService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
            
        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result) return NotFound();

            return Ok("User deleted successfully");
        }

       
        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateUser(string Id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            userDTO.userId = Id;
            var result = await _userService.UpdateUser(userDTO);
            if (!result) return NotFound("User not found.");

            return Ok("User updated successfully.");
        }
       
        [HttpGet("DashboardStats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            var recentOrders = await _context.Orders
                      .OrderByDescending(o => o.OrderDate)
                      .Take(5)
                      .Select(o => new OrderDTO
                      {
                          OrderId = o.OrderId,
                          UserId = o.UserId,
                          OrderDate = o.OrderDate,
                          TotalAmount = o.TotalAmount,
                          Status = o.Status
                      }).ToListAsync();
            var dashboardStats = new DashboardStatsDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                RecentOrders = recentOrders

            };
            return Ok(dashboardStats);
        }

    }
}

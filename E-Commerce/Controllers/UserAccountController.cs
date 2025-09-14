using E_Commerce.Business.Services;
using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataBusiness.Interfaces;
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
        private readonly IUserService _userService;
        

        public UserAccountController(IUserService userService )
        {
            _userService = userService;
           
            
        }

      
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<UserDTO>>> GetPagedUsers(
       [FromQuery] UserPaginationParams parameters)
        {
            var result = await _userService.GetPagedUsersAsync(parameters);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateNewUserDTO newuserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _userService.CreateUserAsync(newuserDTO,newuserDTO.Roles);
            if (!created) return BadRequest("User creation failed");

            var newUser = await _userService.GetUserByUsernameAsync(newuserDTO.UserName);
            return CreatedAtAction(nameof(GetUser), new { id = newUser.userId }, newUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return NotFound();

            return Ok("User deleted successfully");
        }

       
        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateUser(string Id, [FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            userDTO.userId = Id;
            var result = await _userService.UpdateUserAsync(userDTO);
            if (!result) return NotFound("User not found.");

            return Ok("User updated successfully.");
        }
       
        [HttpGet("DashboardStats")]
        public async Task<ActionResult> GetDashboardStats()
        {
            var dashboardStats = await _userService.GetDashboardStatsAsync();
            return Ok(dashboardStats);
        }

    }
}

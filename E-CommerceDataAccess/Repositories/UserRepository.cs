using E_CommerceDataAccess.Data;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public UserRepository(UserManager<UserAccount> userManager,
                            RoleManager<IdentityRole> roleManager,
                            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDTO
            {
                userId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Roles = roles.ToString()
            };
        }

        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDTO
            {
                userId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Roles = roles.ToString()
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
             

                userDtos.Add(new UserDTO
                {
                    userId = user.Id,
                    Phone = user.PhoneNumber,
                    UserName = user.UserName,
                    Email = user.Email,
                    Password = user.PasswordHash,
                    Roles = roles.ToString()
                });
            }

            return userDtos;
        }

        public async Task<bool> CreateUserAsync(UserDTO userDto, string role = null)
        {
            var userAccount = new UserAccount
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.Phone,
                
            };

            var result = await _userManager.CreateAsync(userAccount, userDto.Password);

            if (result.Succeeded && !string.IsNullOrEmpty(role))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                await _userManager.AddToRoleAsync(userAccount, role);
                return true;
            }

            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(UserDTO userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.userId);
            if (user == null) return false;

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return false;

            //if (userDto.Roles != null && userDto.Roles.Any())
            //{
            //    var currentRoles = await _userManager.GetRolesAsync(user);
            //    var rolesToAdd = userDto.Roles.Except(currentRoles);
            //    var rolesToRemove = currentRoles.Except(userDto.Roles);

            //    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            //    await _userManager.AddToRolesAsync(user, rolesToAdd);
            //}

            return true;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }
    }
}
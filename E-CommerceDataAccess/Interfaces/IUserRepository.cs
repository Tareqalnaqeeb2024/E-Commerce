using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<UserDTO> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<bool> CreateUserAsync(UserDTO user, string Role);
        Task<bool> UpdateUserAsync(UserDTO user);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> UserExistsAsync(string username);
      
    }
}

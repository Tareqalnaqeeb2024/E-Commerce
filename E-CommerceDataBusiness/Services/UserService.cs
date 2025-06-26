using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService  _emailService;

        public UserService(IUserRepository userRepository , IEmailService  emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<UserDTO> GetUserByUsername(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<bool> CreateUser(UserDTO user, string role = "User")
        {
            if (await _userRepository.UserExistsAsync(user.UserName))
            {
                return false;
            }

            var created = await _userRepository.CreateUserAsync(user, role);
            if (created)
            {
                await _emailService.SendEmailAsync(
                    user.Email,
                    "مرحبًا بك في تطبيقنا!",
                    $"مرحبًا {user.UserName} 👋\n\nنحن سعداء بانضمامك إلى منصتنا."
                );
            }

            return created;
        }

        public async Task<bool> UpdateUser(UserDTO user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUser(string id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }
    }
}
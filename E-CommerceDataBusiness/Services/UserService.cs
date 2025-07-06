using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService  _emailService;
        private readonly IRedisService _redisCache;
        private const string UserCachePrefix = "user:";
        private const string AllUsersCacheKey = "users:all";


        public UserService(IUserRepository userRepository , IEmailService  emailService , IRedisService redis)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _redisCache = redis;
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            string cacheKey = $"{UserCachePrefix}{id}";

            // Try cache first
            var cachedUser = await _redisCache.GetAsync<UserDTO>(cacheKey);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;
            await _redisCache.SetAsync(cacheKey, user, TimeSpan.FromMinutes(30));
            return user;
        }

        public async Task<UserDTO> GetUserByUsername(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var cachedUsers = await _redisCache.GetAsync<IEnumerable<UserDTO>>(AllUsersCacheKey);
            if (cachedUsers != null)
            {
                return cachedUsers;
            }
            var users = await _userRepository.GetAllUsersAsync();
            await _redisCache.SetAsync(AllUsersCacheKey, users, TimeSpan.FromMinutes(30));
            return users;
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
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
           return await _userRepository.GetDashboardStatsAsync();
        }
    }
}
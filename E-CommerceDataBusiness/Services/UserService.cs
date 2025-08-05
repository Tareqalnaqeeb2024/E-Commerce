using AutoMapper;
using E_CommerceDataAccess.DTO;
using E_CommerceDataAccess.DTO.Common;
using E_CommerceDataAccess.DTO.Pagination;
using E_CommerceDataAccess.Interfaces;
using E_CommerceDataAccess.Models;
using E_CommerceDataAccess.UnitOfWork;
using E_CommerceDataBusiness.Interfaces;
using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services
{
    public class UserService : IUserService
    {
        
        private readonly IUnitOfwork _unitOfwork;
        private readonly IEmailService _emailService;
        private readonly IRedisService _redisCache;
        private readonly IMapper _mapper;
        private const string UserCachePrefix = "user:";
        private const string AllUsersCacheKey = "users:all";
        private const string DashboardCacheKey = "dashboard:stats";

        public UserService(
            IUnitOfwork unitOfwork,
            IEmailService emailService,
            IRedisService redisCache,
            IMapper mapper)
        {
            _unitOfwork = unitOfwork;
            _emailService = emailService;
            _redisCache = redisCache;
            _mapper = mapper;
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            string cacheKey = $"{UserCachePrefix}{id}";
            var cachedUser = await _redisCache.GetAsync<UserDTO>(cacheKey);
            if (cachedUser != null) return cachedUser;

            var user = await _unitOfwork.users.GetByIdAsync(id);
            if (user == null) return null;

            var roles = await _unitOfwork.users.GetUserRolesAsync(id);
            var userDto = _mapper.Map<UserDTO>(user);
            userDto.Roles = string.Join(",", roles);

            await _redisCache.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(30));
            return userDto;
        }

        public async Task<UserDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _unitOfwork.users.GetByUsernameAsync(username);
            if (user == null) return null;

            var roles = await _unitOfwork.users.GetUserRolesAsync(user.Id);
            var userDto = _mapper.Map<UserDTO>(user);
            userDto.Roles = string.Join(",", roles);

            return userDto;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var cachedUsers = await _redisCache.GetAsync<IEnumerable<UserDTO>>(AllUsersCacheKey);
            if (cachedUsers != null) return cachedUsers;

            var users = await       _unitOfwork.users.GetAllAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _unitOfwork.users.GetUserRolesAsync(user.Id);
                userDtos.Add(new UserDTO
                {
                    userId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Roles = string.Join(",", roles)
                });
            }

            await _redisCache.SetAsync(AllUsersCacheKey, userDtos, TimeSpan.FromMinutes(30));
            return userDtos;
        }

        public async Task<bool> CreateUserAsync(UserDTO userDto, string role)
        {
            if (await _unitOfwork.users.ExistsAsync(userDto.UserName))
                return false;

            var userAccount = _mapper.Map<UserAccount>(userDto);
            var created = await _unitOfwork.users.CreateAsync(userAccount, userDto.Password, role);

            if (created)
            {
                await _emailService.SendEmailAsync(
                    userDto.Email,
                    "Welcome to Our Platform",
                    $"Hello {userDto.UserName},\n\nYour account has been successfully created!");

                // Clear cache
                await _redisCache.RemoveAsync(AllUsersCacheKey);
            }

            return created;
        }

        public async Task<bool> UpdateUserAsync(UserDTO userDto)
        {
            var userAccount = await _unitOfwork.users.GetByIdAsync(userDto.userId);
            if (userAccount == null) return false;

            _mapper.Map(userDto, userAccount);
            var updated = await _unitOfwork.users.UpdateAsync(userAccount);

            if (updated && !string.IsNullOrEmpty(userDto.Roles))
            {
                var roles = userDto.Roles.Split(',').ToList();
                await _unitOfwork.users.UpdateUserRolesAsync(userDto.userId, roles);
            }

            if (updated)
            {
                // Clear relevant caches
                await _redisCache.RemoveAsync($"{UserCachePrefix}{userDto.userId}");
                await _redisCache.RemoveAsync(AllUsersCacheKey);
            }

            return updated;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var deleted = await _unitOfwork.users.DeleteAsync(id);
            if (deleted)
            {
                // Clear relevant caches
                await _redisCache.RemoveAsync($"{UserCachePrefix}{id}");
                await _redisCache.RemoveAsync(AllUsersCacheKey);
                await _redisCache.RemoveAsync(DashboardCacheKey);
            }
            return deleted;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _unitOfwork.users.ExistsAsync(username);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var cachedStats = await _redisCache.GetAsync<DashboardStatsDto>(DashboardCacheKey);
            if (cachedStats != null) return cachedStats;

            var stats = await _unitOfwork.users.GetDashboardStatsAsync();
            await _redisCache.SetAsync(DashboardCacheKey, stats, TimeSpan.FromMinutes(15));

            return stats;
        }

        public async Task<PagedResult<UserDTO>> GetPagedUsersAsync(UserPaginationParams parameters)
        {
            string cacheKey = $"users:paged:{JsonConvert.SerializeObject(parameters)}";
            var cachedResult = await _redisCache.GetAsync<PagedResult<UserDTO>>(cacheKey);
            if (cachedResult != null) return cachedResult;

            var pagedResult = await _unitOfwork.users.GetPagedUsersAsync(parameters);
            var userDtos = new List<UserDTO>();

            foreach (var user in pagedResult.Items)
            {
                var roles = await _unitOfwork.users.GetUserRolesAsync(user.Id);
                userDtos.Add(new UserDTO
                {
                    userId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Roles = string.Join(",", roles)
                });
            }

            var result = new PagedResult<UserDTO>
            {
                Items = userDtos,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };

            await _redisCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
            return result;
        }

       
    }
}
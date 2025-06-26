using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace E_CommerceDataBusiness.Services.ExternalServices
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public IConnectionMultiplexer Redis { get; }

        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
            Redis = redis;
        }

        public async Task SetOtpAsync(string email, string otp, TimeSpan expiration)
        {
            await _database.StringSetAsync(email, otp, expiration);
        }

        public async Task<string> GetOtpAsync(string email)
        {
            return await _database.StringGetAsync(email);
        }

        public async Task RemoveOtpAsync(string email)
        {
            await _database.KeyDeleteAsync(email);
        }
    }
}

using E_CommerceDataBusiness.Interfaces.ExternalInterface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace E_CommerceDataBusiness.Services.ExternalServices
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private const string PagedProductKeysSet = "product:paged:keys";


        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
           
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _database.StringGetAsync(key);
            return json.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(json);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}

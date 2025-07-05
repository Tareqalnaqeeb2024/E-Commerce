using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces.ExternalInterface
{
    public interface IRedisService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}

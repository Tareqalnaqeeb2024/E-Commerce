using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces.ExternalInterface
{
    public interface IRedisService
    {
        Task SetOtpAsync(string email, string otp, TimeSpan expiration);
        Task<string> GetOtpAsync(string email);
        Task RemoveOtpAsync(string email);
    }
}

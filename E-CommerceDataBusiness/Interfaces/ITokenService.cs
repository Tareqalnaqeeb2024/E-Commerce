using E_CommerceDataAccess.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(UserAccount user);
    }
}

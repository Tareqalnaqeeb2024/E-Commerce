using E_Commerce.Basic;
using E_CommerceDataAccess.Models;
using E_CommerceDataBusiness.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceDataBusiness.Services.ExternalServices
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserAccount> _userManager;
        //private readonly JwtSettings  _jwtSettings;

        public TokenService(IConfiguration configuration, UserManager<UserAccount> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            //_jwtSettings = jwtSettings;
        }

        public async Task<string> GenerateTokenAsync(UserAccount user)
        {
            var Claims = new List<Claim>();

            Claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            Claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var roles = await _userManager.GetRolesAsync(user);

            Claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));



            //second Get SigningCredentials

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // third initial the token 

            var token = new JwtSecurityToken(
                claims: Claims,
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: sc);

            //var _token = new
            //{
            //    token = new JwtSecurityTokenHandler().WriteToken(token),
            //    expiration = token.ValidTo,
            //    userId = user.Id
            //};


            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }
}

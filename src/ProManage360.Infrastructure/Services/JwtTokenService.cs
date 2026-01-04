//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using ProManage360.Application.Common.Interfaces;
//using ProManage360.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProManage360.Infrastructure.Services
//{
//    public class JwtService : IJwtTokenService
//    {
//        private readonly IConfiguration _config;

//        public JwtService(IConfiguration config)
//        {
//            _config = config;
//        }

//        public string GenerateToken(User user, List<string> roles)
//        {
//            var claims = new List<Claim>
//        {
//            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
//            new Claim(JwtRegisteredClaimNames.Email, user.Email),
//            new Claim("TenantId", user.TenantId.ToString())
//        };

//            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Name, r)));

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _config["JwtSettings:Issuer"],
//                audience: _config["JwtSettings:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(8),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}

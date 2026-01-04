namespace ProManage360.Infrastructure.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(Dictionary<string, string> claims)
    {
        var secretKey = _configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured in appsettings.json");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claimsList = claims.Select(kvp => new Claim(kvp.Key, kvp.Value)).ToList();

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claimsList,
            expires: DateTime.UtcNow.AddMinutes(15), // Access token: 15 minutes
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var secret = _configuration["Jwt:AccessSecret"]!;
        var expiresInMinutes = int.Parse(_configuration["Jwt:AccessExpiresInMinutes"]!);

        return GenerateToken(user, secret, TimeSpan.FromMinutes(expiresInMinutes));
    }

    public string GenerateRefreshToken(User user)
    {
        var secret = _configuration["Jwt:RefreshSecret"]!;
        var expiresInDays = int.Parse(_configuration["Jwt:RefreshExpiresInDays"]!);

        return GenerateToken(user, secret, TimeSpan.FromDays(expiresInDays));
    }

    private string GenerateToken(User user, string secret, TimeSpan lifetime)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(lifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

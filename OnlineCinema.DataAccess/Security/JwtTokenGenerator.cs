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
using Microsoft.AspNetCore.Identity;

namespace OnlineCinema.DataAccess.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public JwtTokenGenerator(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> GenerateAccessToken(User user)
    {
        var secret = _configuration["Jwt:AccessSecret"]!;
        var expiresInMinutes = int.Parse(_configuration["Jwt:AccessExpiresInMinutes"]!);

        return await GenerateToken(user, secret, TimeSpan.FromMinutes(expiresInMinutes));
    }

    public async Task<string> GenerateRefreshToken(User user)
    {
        var secret = _configuration["Jwt:RefreshSecret"]!;
        var expiresInDays = int.Parse(_configuration["Jwt:RefreshExpiresInDays"]!);

        return await GenerateToken(user, secret, TimeSpan.FromDays(expiresInDays));
    }

    private async Task<string> GenerateToken(User user, string secret, TimeSpan lifetime)
    {
        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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

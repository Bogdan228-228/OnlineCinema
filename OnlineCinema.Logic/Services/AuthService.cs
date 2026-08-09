using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineCinema.DataAccess;
using OnlineCinema.DataAccess.Security;
using OnlineCinema.Domain.Enums;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Auth;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly OnlineCinemaDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ITokenHasher _tokenHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<User> userManager,
        OnlineCinemaDbContext dbContext,
        IJwtTokenGenerator jwtTokenGenerator,
        ITokenHasher tokenHasher,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
        _tokenHasher = tokenHasher;
        _configuration = configuration;
    }

    public async Task<Guid> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new ConflictException("Користувач з таким email вже зареєстрований");
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }

        await CreateEmailTokenAsync(user.Id, TokenType.Confirmation);

        return user.Id;
    }

    public async Task<TokenPairResponse> LoginAsync(LoginRequest request, string? userAgent)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("Невірний email або пароль");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Невірний email або пароль");
        }

        return await IssueTokenPairAsync(user, userAgent);
    }

    public async Task<TokenPairResponse> RefreshAsync(string refreshToken)
    {
        var userId = GetUserIdFromToken(refreshToken);

        var storedTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && !t.Revoked && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        var matchedToken = storedTokens.FirstOrDefault(t => _tokenHasher.Verify(refreshToken, t.TokenHash));
        if (matchedToken == null)
        {
            throw new UnauthorizedException("Сесію не знайдено, увійдіть повторно");
        }
        matchedToken.Revoked = true;

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new UnauthorizedException("Користувача не знайдено");
        }

        var tokens = await IssueTokenPairAsync(user, matchedToken.UserAgent);
        await _dbContext.SaveChangesAsync();

        return tokens;
    }

    public async Task LogoutAsync(Guid userId, string refreshToken)
    {
        var storedTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && !t.Revoked)
            .ToListAsync();

        var matchedToken = storedTokens.FirstOrDefault(t => _tokenHasher.Verify(refreshToken, t.TokenHash));
        if (matchedToken != null)
        {
            matchedToken.Revoked = true;
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task ConfirmEmailAsync(string token)
    {
        var emailToken = await FindValidEmailTokenAsync(token, TokenType.Confirmation);

        var user = await _userManager.FindByIdAsync(emailToken.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException("Користувача не знайдено");
        }

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        emailToken.Used = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            await CreateEmailTokenAsync(user.Id, TokenType.PasswordReset);
        }
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var emailToken = await FindValidEmailTokenAsync(token, TokenType.PasswordReset);

        var user = await _userManager.FindByIdAsync(emailToken.UserId.ToString());
        if (user == null)
        {
            throw new NotFoundException("Користувача не знайдено");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }

        emailToken.Used = true;

        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == user.Id && !t.Revoked)
            .ToListAsync();
        foreach (var activeToken in activeTokens)
        {
            activeToken.Revoked = true;
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<TokenPairResponse> IssueTokenPairAsync(User user, string? userAgent)
    {
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user);

        var refreshExpiresInDays = int.Parse(_configuration["Jwt:RefreshExpiresInDays"]!);

        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenHasher.Hash(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiresInDays),
            UserAgent = userAgent
        });

        await _dbContext.SaveChangesAsync();

        return new TokenPairResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private async Task CreateEmailTokenAsync(Guid userId, TokenType type)
    {
        var ttlHours = 24;
        var token = Guid.NewGuid().ToString("N");

        _dbContext.EmailTokens.Add(new EmailToken
        {
            UserId = userId,
            Token = token,
            Type = type,
            ExpiresAt = DateTime.UtcNow.AddHours(ttlHours)
        });

        await _dbContext.SaveChangesAsync();

    }

    private async Task<EmailToken> FindValidEmailTokenAsync(string token, TokenType type)
    {
        var emailToken = await _dbContext.EmailTokens
            .FirstOrDefaultAsync(t => t.Token == token && t.Type == type);

        if (emailToken == null || emailToken.Used)
        {
            throw new ConflictException("Токен недійсний або вже використаний");
        }

        if (emailToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new ConflictException("Термін дії токена сплив");
        }

        return emailToken;
    }

    private Guid GetUserIdFromToken(string refreshToken)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

        if (!handler.CanReadToken(refreshToken))
        {
            throw new UnauthorizedException("Refresh token недійсний");
        }

        var jwtToken = handler.ReadJwtToken(refreshToken);
        var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");

        if (subClaim == null || !Guid.TryParse(subClaim.Value, out var userId))
        {
            throw new UnauthorizedException("Refresh token недійсний");
        }

        return userId;
    }
}

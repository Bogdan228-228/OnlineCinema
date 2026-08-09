using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OnlineCinema.Logic.DTOs.Auth;

namespace OnlineCinema.Logic.Interfaces;

public interface IAuthService
{
    Task<Guid> RegisterAsync(RegisterRequest request);
    Task<TokenPairResponse> LoginAsync(LoginRequest request, string? userAgent);
    Task<TokenPairResponse> RefreshAsync(string refreshToken);
    Task LogoutAsync(Guid userId, string refreshToken);
    Task ConfirmEmailAsync(string token);
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(string token, string newPassword);
}

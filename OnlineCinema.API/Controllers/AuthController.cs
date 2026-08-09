using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Logic.DTOs.Auth;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var userId = await _authService.RegisterAsync(request);
        return Ok(new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var tokens = await _authService.LoginAsync(request, userAgent);
        return Ok(tokens);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var tokens = await _authService.RefreshAsync(request.RefreshToken);
        return Ok(tokens);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        await _authService.LogoutAsync(userId, request.RefreshToken);
        return Ok(new { message = "Вихід виконано" });
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        await _authService.ConfirmEmailAsync(request.Token);
        return Ok(new { message = "Email підтверджено" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request.Email);
        return Ok(new { message = "Якщо такий email існує, на нього надіслано інструкцію" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
        return Ok(new { message = "Пароль оновлено" });
    }
}
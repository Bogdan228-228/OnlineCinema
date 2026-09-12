using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Logic.DTOs.Users;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();
        var profile = await _userService.GetProfileAsync(userId);
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request)
    {
        var userId = GetUserId();
        var profile = await _userService.UpdateProfileAsync(userId, request);
        return Ok(profile);
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = GetUserId();
        await _userService.ChangePasswordAsync(userId, request);
        return Ok(new { message = "Пароль успішно змінено" });
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeactivateMe()
    {
        var userId = GetUserId();
        await _userService.DeactivateAsync(userId);
        return Ok(new { message = "Акаунт деактивовано" });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
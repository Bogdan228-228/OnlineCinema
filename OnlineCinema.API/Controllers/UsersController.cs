using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Logic.DTOs.Users;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserActivityService _userActivityService;

    public UsersController(IUserService userService, IUserActivityService userActivityService)
    {
        _userService = userService;
        _userActivityService = userActivityService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();
        var profile = await _userService.GetProfileAsync(userId);
        await _userActivityService.AddActivityAsync(userId, userId.ToString(), Domain.Enums.EntityType.User, Domain.Enums.ActionType.View);
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request, IFormFile? image)
    {
        var userId = GetUserId();
        var profile = await _userService.UpdateProfileAsync(userId, request, image);
        await _userActivityService.AddActivityAsync(userId, userId.ToString(), Domain.Enums.EntityType.User, Domain.Enums.ActionType.Put);
        return Ok(profile);
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = GetUserId();
        await _userService.ChangePasswordAsync(userId, request);
        await _userActivityService.AddActivityAsync(userId, userId.ToString(), Domain.Enums.EntityType.User, Domain.Enums.ActionType.Patch);
        return Ok(new { message = "Пароль успішно змінено" });
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeactivateMe()
    {
        var userId = GetUserId();
        await _userService.DeactivateAsync(userId);
        await _userActivityService.AddActivityAsync(userId, userId.ToString(), Domain.Enums.EntityType.User, Domain.Enums.ActionType.Delete, metadata: "Deactivated");
        return Ok(new { message = "Акаунт деактивовано" });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
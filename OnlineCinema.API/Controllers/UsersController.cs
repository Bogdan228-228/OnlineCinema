using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Domain.Abstractions.Services;
using OnlineCinema.Logic.DTOs.Users;
using OnlineCinema.Logic.Interfaces;
using System.Security.Claims;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserActivityService _userActivityService;
    private readonly IUserStatistic _userStatistic;

    public UsersController(IUserService userService, IUserActivityService userActivityService, IUserStatistic userStatistic)
    {
        _userService = userService;
        _userActivityService = userActivityService;
        _userStatistic = userStatistic;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetUserId();
        var profile = await _userService.GetProfileAsync(userId);
        await _userActivityService.AddActivityAsync(userId, userId.ToString(), Domain.Enums.EntityType.User, Domain.Enums.ActionType.View);
        return Ok(profile);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetMyStats()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var stats = await _userStatistic.GetUserStatisticAsync(userId);
        return Ok(stats);
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCinema.Domain.Constants;
using OnlineCinema.Logic.DTOs.Roles;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.API.Controllers;

[ApiController]
[Authorize(Roles = RoleNames.Admin)]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        return Ok(roles);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(AssignRoleRequest request)
    {
        await _roleService.AssignRoleAsync(request);
        return Ok(new { message = "Роль призначено" });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeRole(AssignRoleRequest request)
    {
        await _roleService.RevokeRoleAsync(request);
        return Ok(new { message = "Роль знято" });
    }
}

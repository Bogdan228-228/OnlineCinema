using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Roles;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleService(RoleManager<Role> roleManager, UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public Task<IReadOnlyList<RoleResponse>> GetAllRolesAsync()
    {
        var roles = _roleManager.Roles
            .Select(r => new RoleResponse
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<RoleResponse>>(roles);
    }

    public async Task<UserRolesResponse> GetUserRolesAsync(Guid userId)
    {
        var user = await FindUserOrThrowAsync(userId);
        var roles = await _userManager.GetRolesAsync(user);

        return new UserRolesResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
    }

    public async Task AssignRoleAsync(AssignRoleRequest request)
    {
        var user = await FindUserOrThrowAsync(request.UserId);

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
        {
            throw new NotFoundException("Роль не знайдено");
        }

        var alreadyInRole = await _userManager.IsInRoleAsync(user, request.RoleName);
        if (alreadyInRole)
        {
            throw new ConflictException("Користувач вже має цю роль");
        }

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }
    }

    public async Task RevokeRoleAsync(AssignRoleRequest request)
    {
        var user = await FindUserOrThrowAsync(request.UserId);

        var alreadyInRole = await _userManager.IsInRoleAsync(user, request.RoleName);
        if (!alreadyInRole)
        {
            throw new ConflictException("Користувач не має цієї ролі");
        }

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }
    }

    private async Task<User> FindUserOrThrowAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new NotFoundException("Користувача не знайдено");
        }

        return user;
    }
}

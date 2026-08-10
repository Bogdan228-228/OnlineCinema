using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Users;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserResponse> GetProfileAsync(Guid userId)
    {
        var user = await FindUserOrThrowAsync(userId);
        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await FindUserOrThrowAsync(userId);

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            user.AvatarUrl = request.AvatarUrl;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }

        return MapToResponse(user);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await FindUserOrThrowAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }
    }

    public async Task DeactivateAsync(Guid userId)
    {
        var user = await FindUserOrThrowAsync(userId);

        user.IsActive = false;
        await _userManager.UpdateAsync(user);
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

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt
        };
    }
}
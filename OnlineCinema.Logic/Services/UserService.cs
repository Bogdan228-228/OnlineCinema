using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OnlineCinema.Domain.Models;
using OnlineCinema.Logic.DTOs.Users;
using OnlineCinema.Logic.Exceptions;
using OnlineCinema.Logic.Interfaces;

namespace OnlineCinema.Logic.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly BlobServiceClient _blobServiceClient;

    public UserService(UserManager<User> userManager, BlobServiceClient blobServiceClient)
    {
        _userManager = userManager;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<UserResponse> GetProfileAsync(Guid userId)
    {
        var user = await FindUserOrThrowAsync(userId);
        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, IFormFile? image)
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

        if (image != null)
        {
            user = await UploadUserImageAsync(userId, image!);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }

        return MapToResponse(user);
    }

    public async Task<User> UploadUserImageAsync(Guid userId, IFormFile file)
    {
        var user = await FindUserOrThrowAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var containerClient = _blobServiceClient.GetBlobContainerClient("public-assets");
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"avatars/users/{user.Id}/{file.FileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        user.AvatarUrl = blobClient.Uri.ToString();

        return user;
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
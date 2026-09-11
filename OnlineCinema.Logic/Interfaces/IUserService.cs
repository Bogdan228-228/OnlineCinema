using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.Users;

namespace OnlineCinema.Logic.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetProfileAsync(Guid userId);
    Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task DeactivateAsync(Guid userId);
}
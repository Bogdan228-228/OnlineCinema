using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineCinema.Logic.DTOs.Roles;

namespace OnlineCinema.Logic.Interfaces;

public interface IRoleService
{
    Task<IReadOnlyList<RoleResponse>> GetAllRolesAsync();
    Task<UserRolesResponse> GetUserRolesAsync(Guid userId);
    Task AssignRoleAsync(AssignRoleRequest request);
    Task RevokeRoleAsync(AssignRoleRequest request);
}

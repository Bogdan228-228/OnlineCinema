using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineCinema.Logic.DTOs.Roles;

public class AssignRoleRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string RoleName { get; set; } = null!;
}

using System;

namespace OnlineCinema.Logic.DTOs.Roles;

public class RoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

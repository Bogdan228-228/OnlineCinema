using System;
using System.Collections.Generic;

namespace OnlineCinema.Logic.DTOs.Roles;

public class UserRolesResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public IReadOnlyList<string> Roles { get; set; } = new List<string>();
}

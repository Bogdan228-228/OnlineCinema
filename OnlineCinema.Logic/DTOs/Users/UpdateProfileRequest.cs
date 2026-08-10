using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCinema.Logic.DTOs.Users;

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
}
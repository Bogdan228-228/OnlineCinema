using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCinema.Logic.DTOs.Users;

public class UpdateProfileRequest
{
    [StringLength(200)]
    public string? FullName { get; set; }

    [StringLength(500)]
    public string? AvatarUrl { get; set; }
}

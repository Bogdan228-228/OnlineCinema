
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCinema.Logic.DTOs.Auth;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = null!;
}

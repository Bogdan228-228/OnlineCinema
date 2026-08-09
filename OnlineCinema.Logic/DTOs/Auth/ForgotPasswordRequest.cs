using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCinema.Logic.DTOs.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = null!;
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Security;

public interface IJwtTokenGenerator
{
    Task<string> GenerateAccessToken(User user);
    Task<string> GenerateRefreshToken(User user);
}
using OnlineCinema.Domain.Models;

namespace OnlineCinema.DataAccess.Security;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken(User user);
}

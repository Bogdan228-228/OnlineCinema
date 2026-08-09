using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BCrypt.Net;

namespace OnlineCinema.DataAccess.Security;

public class TokenHasher : ITokenHasher
{
    public string Hash(string rawToken)
    {
        return BCrypt.Net.BCrypt.HashPassword(rawToken);
    }

    public bool Verify(string rawToken, string hashedToken)
    {
        return BCrypt.Net.BCrypt.Verify(rawToken, hashedToken);
    }
}
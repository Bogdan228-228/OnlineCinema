using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCinema.DataAccess.Security;

public interface ITokenHasher
{
    string Hash(string rawToken);
    bool Verify(string rawToken, string hashedToken);
}
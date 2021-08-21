using SimpleAuthServer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshTokenForUser(AuthorizedUser user);
        bool ValidateRefreshToken(string token);
        AuthorizedUser GetUserFromRefreshToken(string token);
    }
}

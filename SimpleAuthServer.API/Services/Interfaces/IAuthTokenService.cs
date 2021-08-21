using Microsoft.AspNetCore.Http;
using SimpleAuthServer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services.Interfaces
{
    public interface IAuthTokenService
    {
        string GenerateAccessTokenForUser(AuthorizedUser user);
        bool IsDeactivated(string token);
        bool IsDeactivated(HttpRequest request);
        void DeactivateToken(string token);
        List<RevokedTokenInfo> GetDeactivatedTokens();
    }
}

using SimpleAuthServer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services.Interfaces
{
    public interface IUserStore
    {
        void StoreUsers(IEnumerable<AuthorizedUser> users);
        List<AuthorizedUser> RetrieveUsers();

        void StoreRefreshTokens(IEnumerable<RefreshToken> tokens);
        List<RefreshToken> RetrieveRefreshTokens();
    }
}

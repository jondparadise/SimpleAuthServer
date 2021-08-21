using SimpleAuthServer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<AuthorizedUser> CreateUser(string username, string password);
        Task<AuthorizedUser> FindUserByUserName(string username);
        Task<AuthorizedUser> UpdateUser(AuthorizedUser user);
        Task<bool> RemoveUser(AuthorizedUser user);
    }
}

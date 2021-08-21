using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services.Interfaces
{
    public interface ICryptoService
    {
        Task<bool> ValidateLogin(string password, string passwordHash);

        Task<string> HashPassword(string password);
    }
}

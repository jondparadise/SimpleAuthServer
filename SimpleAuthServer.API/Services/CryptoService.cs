using Microsoft.Extensions.Configuration;
using SimpleAuthServer.API.Models.Configuration;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly CryptoConfiguration config;

        public CryptoService(CryptoConfiguration configuration)
        {
            this.config = configuration;
        }

        public async Task<string> HashPassword(string password)
        {
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(config.PrivateKey));
            byte[] hashResult;
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(password)))
            {
                hashResult = await hmac.ComputeHashAsync(memStream);
            }

            return Encoding.UTF8.GetString(hashResult);
        }

        public async Task<bool> ValidateLogin(string password, string passwordHash)
        {
            return passwordHash.Equals(await HashPassword(password), StringComparison.Ordinal);
        }
    }
}

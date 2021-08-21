using Microsoft.Extensions.DependencyInjection;
using SimpleAuthServer.API.Models.Configuration;
using SimpleAuthServer.API.Services;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API
{
    public static class ExtensionMethods
    {
        public static void AddCryptoService(this IServiceCollection services)
        {
            services.AddSingleton<ICryptoService, CryptoService>();
        }

        public static void AddTokenService(this IServiceCollection services, TokenConfiguration tokenConfiguration)
        {
            var fileStore = new FileSystemDataService();
            services.AddSingleton<IUserStore>(fileStore);
            var tokenService = new TokenService(tokenConfiguration, fileStore);
            services.AddSingleton(tokenService);
        }

        public static void AddDataRepository(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, InMemoryDataService>();
        }
    }
}

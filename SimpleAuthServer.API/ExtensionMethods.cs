using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleAuthServer.API.Models.Configuration;
using SimpleAuthServer.API.Services;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAuthServer.API
{
    public static class ExtensionMethods
    {
        public static void AddSimpleAuth(this IServiceCollection services, TokenConfiguration tokenConfiguration, FileStorageConfiguration fileStorageConfiguration, CryptoConfiguration cryptoConfiguration)
        {
            //Configurations
            services.AddSingleton(tokenConfiguration);
            services.AddSingleton(fileStorageConfiguration);
            services.AddSingleton(cryptoConfiguration);
            
            //ICryptoService
            services.AddSingleton<ICryptoService, CryptoService>();

            //IUserStore
            services.AddSingleton<IUserStore, FileSystemDataService>();

            //IDataRepository
            services.AddSingleton<IUserRepository, InMemoryDataService>();

            //IAuthTokenService, IRefreshTokenService
            services.AddSingleton<TokenService>();

            //Configure JwtToken
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(token =>
                {
                    token.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.TokenSecret)),
                        ValidIssuer = tokenConfiguration.Issuer,
                        ValidAudience = tokenConfiguration.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });
        }
    }
}

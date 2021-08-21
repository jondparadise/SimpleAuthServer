using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SimpleAuthServer.API.Models;
using SimpleAuthServer.API.Models.Configuration;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services
{
    public class TokenService : IAuthTokenService, IRefreshTokenService
    {
        private readonly TokenConfiguration config;
        private readonly IUserStore userStore;
        private List<RevokedTokenInfo> DeactivatedTokens { get; set; } = new List<RevokedTokenInfo>();
        private List<RefreshToken> RefreshTokens { get; set; }

        public TokenService(TokenConfiguration tokenConfiguration, IUserStore userStore)
        {
            this.config = tokenConfiguration;
            this.userStore = userStore;
            this.RefreshTokens = this.userStore.RetrieveRefreshTokens() ?? new List<RefreshToken>();
        }

        public string GenerateAccessTokenForUser(AuthorizedUser user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.TokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                config.Issuer, 
                config.Audience,
                GenerateUserClaims(user),
                DateTime.Now, 
                DateTime.Now.AddMinutes(config.TokenExpirationInMinutes),
                credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshTokenForUser(AuthorizedUser user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.RefreshTokenSecret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            JwtSecurityToken token = new JwtSecurityToken(
                config.Issuer,
                config.Audience,
                GenerateUserClaims(user),
                DateTime.Now,
                DateTime.Now.AddMinutes(config.RefreshTokenExpirationInMinutes),
                credentials);

            RefreshToken newToken = new RefreshToken() { 
                Expiration = token.ValidTo, 
                IsActive = true, 
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserID = user.ID,
                Username = user.Username
            };

            this.RefreshTokens.Add(newToken);
            
            return newToken.Token;
        }

        private List<Claim> GenerateUserClaims(AuthorizedUser user)
        {
            return new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
        }

        public AuthorizedUser GetUserFromRefreshToken(string token)
        {
            SecurityToken securityToken;
            var validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.RefreshTokenSecret)),
                ValidIssuer = config.Issuer,
                ValidAudience = config.Audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
            
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out securityToken);
            var userID = principal.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier).Value;
            var username = principal.Claims.FirstOrDefault(item => item.Type == ClaimTypes.Name).Value;

            return new AuthorizedUser() { ID = new Guid(userID), Username = username };
        }

        private bool PurgeRefreshTokens()
        {
            if(RefreshTokens != null && RefreshTokens.Count > 0)
            {
                var expiredTokens = RefreshTokens.Where(item => item.Expiration < DateTime.Now).ToList();
                if(expiredTokens != null && expiredTokens.Count > 0)
                {
                    foreach(var t in expiredTokens)
                    {
                        RefreshTokens.Remove(t);
                    }

                    return true;
                }
            }

            return false;
        }

        public string GetRefreshTokenForUser(AuthorizedUser user)
        {
            var retVal = string.Empty;
            var needsSaved = this.PurgeRefreshTokens();
            var matchingToken = RefreshTokens.Where(item => item.UserID == user.ID).OrderByDescending(order => order.Expiration).FirstOrDefault();
            
            if(matchingToken != null)
            {
                retVal = matchingToken.Token;
            }
            else
            {
                needsSaved = true;
                retVal = this.GenerateRefreshTokenForUser(user);
            }

            if (needsSaved)
            {
                this.userStore.StoreRefreshTokens(this.RefreshTokens);
            }

            return retVal;
        }

        public void DeactivateToken(string token)
        {
            this.DeactivatedTokens.Add(new RevokedTokenInfo() { 
                Token = token, 
                RemoveAt = DateTime.Now.AddMinutes(config.TokenExpirationInMinutes) 
            });
        }

        public bool IsDeactivated(HttpRequest request)
        {
            var token = request.Headers["Authorization"];
            return string.IsNullOrWhiteSpace(token) ? true : IsDeactivated(token);
        }

        public bool IsDeactivated(string token)
        {
            bool retVal = false;
            var match = DeactivatedTokens.FirstOrDefault(item => item.Token.Equals(token, StringComparison.Ordinal));
            
            if(match != null)
            {
                retVal = true;
            }

            var readyForRemoval = DeactivatedTokens.Where(item => item.RemoveAt < DateTime.Now).ToList();
            if(readyForRemoval != null && readyForRemoval.Count > 0)
            {
                foreach(var t in readyForRemoval)
                {
                    this.DeactivatedTokens.Remove(t);
                }
            }

            return retVal;
        }

        public List<RevokedTokenInfo> GetDeactivatedTokens()
        {
            return DeactivatedTokens;
        }

        public bool ValidateRefreshToken(string token)
        {
            this.PurgeRefreshTokens();
            return RefreshTokens.Count(item => item.Token.Equals(token, StringComparison.Ordinal) && item.IsActive) > 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Models.Configuration
{
    public class TokenConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpirationInMinutes { get; set; }
        public string TokenSecret { get; set; }
        public int RefreshTokenExpirationInMinutes { get; set; }
        public string RefreshTokenSecret { get; set; }
    }
}

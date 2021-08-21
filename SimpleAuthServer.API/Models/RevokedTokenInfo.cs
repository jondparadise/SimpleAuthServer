using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Models
{
    public class RevokedTokenInfo
    {
        public string Token { get; set; }
        public DateTime RemoveAt { get; set; }
    }
}

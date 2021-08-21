using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Models
{
    public class RefreshToken
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public DateTime Expiration { get; set; }
        public string Token { get; set; }
        public bool IsActive { get; set; }
    }
}

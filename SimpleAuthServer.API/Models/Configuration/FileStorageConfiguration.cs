using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Models.Configuration
{
    public class FileStorageConfiguration
    {
        public string RefreshTokensFilepath { get; set; }
        public string UsersFilepath { get; set; }
    }
}

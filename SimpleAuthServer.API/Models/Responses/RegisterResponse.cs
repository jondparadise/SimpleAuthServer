using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Models.Responses
{
    public class RegisterResponse : BaseResponse
    {
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

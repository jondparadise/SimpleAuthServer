using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Models.Requests
{
    public class RegisterRequest
    {
        [Required, DataType(DataType.Text)]
        public string Username { get; set; }
        
        [Required, MinLength(8), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, MinLength(8), DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}

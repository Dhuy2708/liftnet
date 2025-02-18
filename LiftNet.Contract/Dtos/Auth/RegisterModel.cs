using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Auth
{
    public class RegisterModel
    {
        public required string Name
        {
            get; set;
        }
        public required string Email
        {
            get; set;
        }
        public required string Username
        {
            get; set;
        }
        public required string Password
        {
            get; set;
        }
        public string? PhoneNumber
        {
            get; set;
        }

    }
}

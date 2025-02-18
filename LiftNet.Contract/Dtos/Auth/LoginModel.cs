using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Auth
{
    public class LoginModel
    {
        [Required]
        public required string Username
        {
            get; set;
        }
        [Required]
        public required string Password
        {
            get; set;
        }
    }
}

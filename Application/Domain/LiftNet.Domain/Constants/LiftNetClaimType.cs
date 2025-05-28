using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Constants
{
    public static class LiftNetClaimType
    {
        public static string UId => ClaimTypes.NameIdentifier;
        public static string UEmail => ClaimTypes.Email;
        public static string Username => ClaimTypes.Name;
        public static string FirstName => "firstname";
        public static string LastName => "lastname";
        public static string Roles => ClaimTypes.Role;
        public static string UAvatar => "uavatar";
    }
}

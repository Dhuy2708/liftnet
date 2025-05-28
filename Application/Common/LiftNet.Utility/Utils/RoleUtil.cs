using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public class RoleUtil
    {
        public static LiftNetRoleEnum GetRole(string role)
        {
            return role switch
            {
                "Seeker" => LiftNetRoleEnum.Seeker,
                "Coach" => LiftNetRoleEnum.Coach,
                "Admin" => LiftNetRoleEnum.Admin,
                _ => LiftNetRoleEnum.None,
            };
        }
    }
}

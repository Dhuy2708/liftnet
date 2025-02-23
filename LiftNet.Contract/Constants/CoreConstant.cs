using LiftNet.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Constants
{
    public class CoreConstant
    {
        // credentials
        public const string DEFAULT_USER_AVATAR = DomainConstants.DEFAULT_USER_AVATAR;
        public const int TokenExpirationTimeInSeconds = 3600 * 24;

        // log
        public const string LOG_KEY = "log";
    }
}

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

    public class CosmosContainerId
    {
        public const string Feed = "feed";
        public const string Comment = "comment";
        public const string Chat = "chat";
    }

    public class EnvKeys
    {
        public const string JWT_KEY = "JWT_KEY";
        public const string JWT_ISSUER = "JWT_ISSUER";
        public const string JWT_AUDIENCE = "JWT_AUDIENCE";
        public const string SQL_CONNECTION_STRING = "SQL_CONNECTION_STRING";
        public const string BLOB_CONNECTION_STRING = "BLOB_CONNECTION_STRING";
        public const string COSMOS_CONNECTION_STRING = "COSMOS_CONNECTION_STRING";
        public const string COSMOS_DATABASE_ID = "COSMOS_DATABASE_ID";
    }
}

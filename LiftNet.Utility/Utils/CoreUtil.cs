using LiftNet.Contract.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public class CoreUtil
    {
        private static CoreUtil _instance;
        private static readonly object _lock = new object();

        private CoreUtil()
        {
        }

        public static CoreUtil Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CoreUtil();
                    }
                }
            }
            return _instance;
        }

        public byte[] GetSecretKey()
        {
            string key = Environment.GetEnvironmentVariable(EnvKeys.JWT_KEY)
                         ?? throw new InvalidOperationException("JWT_KEY environment variable is not set.");
            return Encoding.UTF8.GetBytes(key);
        }

        public string GetIssuer()
        {
            return Environment.GetEnvironmentVariable(EnvKeys.JWT_ISSUER)
                   ?? throw new InvalidOperationException("JWT_ISSUER environment variable is not set.");
        }

        public string GetValidAudience()
        {
            return Environment.GetEnvironmentVariable(EnvKeys.JWT_AUDIENCE)
                   ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable is not set.");
        }
    }
}

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

        // img
        public static (int WIDTH, int HEIGHT) MAXIMUM_IMAGE_RESOLUTION = (500, 500);
    }

    public class LiftNetVersionKeys
    {
        public const string VN_GEO = "VN_GEO_VERSION";
    }

    public class CosmosContainerId
    {
        public const string Feed = "feed";
        public const string Comment = "comment";
        public const string Chat = "chat";
        public const string Schedule = "schedule";
        public const string Address = "address";
    }

    public class BlobContainerName
    {
        public const string FFM_MODEL = "ffmmodel";
    }

    public class EnvKeys
    {
        public const string JWT_KEY = "JWT_KEY";
        public const string JWT_ISSUER = "JWT_ISSUER";
        public const string JWT_AUDIENCE = "JWT_AUDIENCE";
        public const string SQL_CONNECTION_STRING = "SQL_CONNECTION_STRING";
        public const string LOCAL_SQL_CONNECTION = "LOCAL_SQL_CONNECTION";
        public const string BLOB_CONNECTION_STRING = "BLOB_CONNECTION_STRING";
        public const string COSMOS_CONNECTION_STRING = "COSMOS_CONNECTION_STRING";
        public const string COSMOS_DATABASE_ID = "COSMOS_DATABASE_ID";
        public const string GOONG_MAP_API_KEY = "GOONG_MAP_API_KEY";
        public const string EXERCISE_API_KEY = "EXERCISE_API_KEY";
        public const string CLOUDINARY_CLOUD_NAME = "CLOUDINARY_CLOUD_NAME";
        public const string CLOUDINARY_API_KEY = "CLOUDINARY_API_KEY";
        public const string CLOUDINARY_API_SECRET = "CLOUDINARY_API_SECRET";

        // rabbitmq
        public const string DEV_RABBITMQ_HOST_NAME = "DEV_RABBITMQ_HOST_NAME";
        public const string DEV_RABBITMQ_USERNAME = "DEV_RABBITMQ_USERNAME";
        public const string DEV_RABBITMQ_PASSWORD = "DEV_RABBITMQ_PASSWORD";
        public const string DEV_RABBITMQ_URL = "DEV_RABBITMQ_URL";
        public const string DEV_RABBITMQ_PORT = "DEV_RABBITMQ_PORT";

        public const string TEST_RABBITMQ_HOST_NAME = "TEST_RABBITMQ_HOST_NAME";
        public const string TEST_RABBITMQ_USERNAME = "TEST_RABBITMQ_USERNAME";
        public const string TEST_RABBITMQ_PASSWORD = "TEST_RABBITMQ_PASSWORD";
        public const string TEST_RABBITMQ_URL = "TEST_RABBITMQ_URL";
        public const string TEST_RABBITMQ_PORT = "TEST_RABBITMQ_PORT";

        // redis cache
        public const string REDIS_HOST_NAME = "REDIS_HOST_NAME";
        public const string REDIS_PORT = "REDIS_PORT";
        public const string REDIS_USER = "REDIS_USER";
        public const string REDIS_PASSWORD = "REDIS_PASSWORD";

        // chatbot engine
        public const string LOCAL_CHATBOT_ENGINE_URL = "LOCAL_CHATBOT_ENGINE_URL";
        public const string CHATBOT_ENGINE_URL = "CHATBOT_ENGINE_URL";

        // vnpay
        public const string VNP_TMNCODE = "VNP_TMNCODE";
        public const string VNP_HASH_SECRET = "VNP_HASH_SECRET";
        public const string VNP_SANDBOX_URL = "VNP_SANDBOX_URL";
        public const string LOCAL_VNP_CALLBACK_URL = "LOCAL_VNP_CALLBACK_URL";
        public const string VNP_CALLBACK_URL = "VNP_CALLBACK_URL";

    }

    public class RedisCacheKeys
    {
        public const string SEEN_FEEDS_CACHE_KEY = "feed:seen:{0}"; // user id 
        public const string SUGGESTED_FRIENDS_CACHE_KEY = "user:suggested-friends:{0}"; // user id
        public const string EXPLORED_FINDER_POST_CACHE_KEY = "finder:explore:{0}"; // trainer id
    }
}

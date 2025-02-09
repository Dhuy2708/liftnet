using LiftNet.Contract.Constants;
using LiftNet.Logger.Enum;
using LiftNet.Logger.Model;
using LiftNet.Utility.Utils;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Logger.Core
{
    public class LifLogger<T> where T : class
    {
        private static Dictionary<LogType, string> LogTypeMapping = new Dictionary<LogType, string>
        {
            { LogType.INFO, "INFO" },
            { LogType.WARNING, "WARNING" },
            { LogType.ERROR, "ERROR" },
        };
        private string? UserId { get; set; }

        private readonly IMemoryCache _memoryCache;
        public LifLogger(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void SetUserId(string userId)
        {
            UserId = userId;
        }

        private void Log(LogType type, string message)
        {
            UserId ??= ContextUtil.UId();
            string key = "";
            if (UserId != null)
            {
                key = CoreConstant.USER_LOG_PREFIX_KEY + UserId;
            }
            else
            {
                key = CoreConstant.SYSTEM_LOG_KEY;
            }

            var logModel = _memoryCache.Get<LifLogModel>(key);
            logModel ??= new LifLogModel { UserId = UserId! };

            var time = DateTime.UtcNow;
            logModel.Logs.Add(time, GetFormatMessage(time, type, message));

            _memoryCache.Set(key, logModel);
        }

        public void LogInfo(string message)
        {
            Log(LogType.INFO, message);
        }

        public void LogWarning(string message)
        {
            Log(LogType.WARNING, message);
        }

        public void LogError(string message)
        {
            Log(LogType.ERROR, message);
        }

        private string GetFormatMessage(DateTime time, LogType logType, string message)
        {
            var typeStr = LogTypeMapping[logType];
            var logMessage = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{typeStr}] [{GetClassName()}]: {message}";
            return logMessage;
        }

        private string GetClassName()
        {
            return typeof(T).Name;
        }
    }
}

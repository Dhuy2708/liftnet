using LiftNet.Contract.Constants;
using LiftNet.Domain.Interfaces;
using LiftNet.Ioc;
using LiftNet.Logger.Enum;
using LiftNet.Logger.Model;
using LiftNet.Utility.Utils;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Logger.Core
{
    public class LiftLogger<T> : ILiftLogger<T>, IDependency
           where T : class
    {
        private readonly ILogger<T> _logger;
        private readonly IMemoryCache _memoryCache;

        public LiftLogger(ILogger<T> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        private static Dictionary<LogType, string> LogTypeMapping = new Dictionary<LogType, string>
        {
            { LogType.INFO, "INFO" },
            { LogType.WARNING, "WARNING" },
            { LogType.ERROR, "ERROR" },
        };
        private string? UserId { get; set; }

        public void SetUserId(string userId)
        {
            UserId = userId;
        }

        private void Log(LogType type, string message)
        {
            LogConsole(type, message);
            UserId ??= ContextUtil.UId();
            var now = DateTime.UtcNow;
            var logKey = CoreConstant.LOG_KEY;
            var logStr = GetFormatMessage(now, type, message);

            var logModel = _memoryCache.Get<LifLogModel>(logKey);

            if (logModel == null)
            {
                logModel = new LifLogModel();
            }

            if (UserId != null) // user log
            {
                if (logModel.UserLogs.TryGetValue(UserId, out var userLogs))
                {
                    userLogs.TryAdd(now, logStr);
                }
                else
                {
                    logModel.UserLogs.TryAdd(UserId, new SortedDictionary<DateTime, string>() { { now, logStr } });
                }
            }
            else // system log
            {
                logModel.SystemLogs ??= new SortedDictionary<DateTime, string>();
                logModel.SystemLogs.TryAdd(now, logStr);
            }

            _memoryCache.Set(logKey, logModel);
        }

        public void Info(string message)
        {
            Log(LogType.INFO, message);
        }

        public void Warn(string message)
        {
            Log(LogType.WARNING, message);
        }

        public void Error(string message)
        {
            Log(LogType.ERROR, message);
        }

        public void Error(Exception e, string message)
        {
            var newMsg = $"{message}, ex: {e.Message}";
        }

        private void LogConsole(LogType type, string message)
        {
            switch (type)
            {
                case LogType.INFO:
                    _logger.LogInformation(message);
                    break;
                case LogType.WARNING:
                    _logger.LogWarning(message);
                    break;
                case LogType.ERROR:
                    _logger.LogError(message);
                    break;
                default:
                    throw new NotSupportedException("unsupported log type");
            }
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

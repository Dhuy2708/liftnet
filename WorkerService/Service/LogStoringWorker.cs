using Azure.Storage.Blobs.Specialized;
using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.Service;
using LiftNet.Logger.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerService.Service
{
    public class LogStoringWorker : BackgroundService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBlobService _blobService;
        private readonly ILogger<LogStoringWorker> _logger;

        public LogStoringWorker(IMemoryCache memoryCache, IBlobService blobService, ILogger<LogStoringWorker> logger)
        {
            _memoryCache = memoryCache;
            _blobService = blobService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_memoryCache.TryGetValue(CoreConstant.LOG_KEY, out LifLogModel? logModel))
                    {
                        if (logModel == null)
                        {
                            return;
                        }
                        var userLogs = logModel.UserLogs;
                        var systemLogs = logModel.SystemLogs;
                        logModel.Clear();
                        var sb = new StringBuilder();
                        foreach (var userLog in userLogs)
                        {
                            var userId = userLog.Key;
                            _logger.LogInformation($"Handle user log, log line count: {userLog.Value.Values.Count}, userId: {userId}");
                            var startUserLogTime = userLog.Value.First().Key;
                            var userLogPath = $"{startUserLogTime.Year}/{startUserLogTime.Month}/{startUserLogTime.Day}/userLogs.txt";

                            foreach (var logLine in userLog.Value.Values)
                            {
                                sb.AppendLine(logLine);
                            }
                            var userLogLines = sb.ToString();
                            await AppendLogToBlobAsync(userId, userLogPath, userLogLines);
                            sb.Clear();
                        }


                        _logger.LogInformation($"Handle system log, log line count: {systemLogs.Values.Count}");
                        var startSystemLogTime = systemLogs.Keys.First();
                        var systemLogPath = $"{startSystemLogTime.Year}/{startSystemLogTime.Month}/{startSystemLogTime.Day}/userLogs.txt";
                        foreach (var systemLog in systemLogs)
                        {
                            sb.AppendLine(systemLog.Value);
                        }
                        var systemLogLines = sb.ToString();
                        await AppendLogToBlobAsync("system", systemLogPath, systemLogLines);
                        sb.Clear();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LogError while storing logModel to Azure Blob Storage");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task AppendLogToBlobAsync(string containerName, string blobPath, string logs)
        {
            var containerClient = await _blobService.GetContainerClient(containerName);

            var blobClient = containerClient.GetAppendBlobClient(blobPath);

            if (!await blobClient.ExistsAsync())
            {
                await blobClient.CreateAsync();
            }

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            writer.WriteLine(logs);
            writer.Flush();
            memoryStream.Position = 0;

            await blobClient.AppendBlockAsync(memoryStream);
        }
    }

}

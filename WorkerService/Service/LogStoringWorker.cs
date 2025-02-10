using LiftNet.Contract.Interfaces.Service;
using LiftNet.Logger.Model;
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
                    if (_memoryCache.TryGetValue(LogCacheKey, out var logModel))
                    {
                        foreach (var (customerId, logEntries) in logModel)
                        {
                            if (!logEntries.Any()) continue;

                            string logPath = $"{customerId}/{DateTime.UtcNow:yyyy/MM/dd}/log.txt";
                            await AppendLogToBlobAsync(logPath, logEntries);
                            logEntries.Clear(); // Xóa log sau khi ghi
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while storing logModel to Azure Blob Storage");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task AppendLogToBlobAsync(string blobPath, SortedDictionary<DateTime, string> logEntries)
        {
            var containerClient = _blobService.GetBlobContainerClient("logModel");
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobPath);

            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);

            foreach (var (timestamp, log) in logEntries)
            {
                await writer.WriteLineAsync($"[{timestamp:yyyy-MM-dd HH:mm:ss}] {log}");
            }

            await writer.FlushAsync();
            memoryStream.Position = 0;

            if (await blobClient.ExistsAsync())
            {
                var existingContent = await blobClient.DownloadContentAsync();
                var newContent = existingContent.Value.Content.ToStream();
                memoryStream.Position = 0;
                newContent.Position = 0;

                using var finalStream = new MemoryStream();
                await newContent.CopyToAsync(finalStream);
                await memoryStream.CopyToAsync(finalStream);
                finalStream.Position = 0;
                await blobClient.UploadAsync(finalStream, overwrite: true);
            }
            else
            {
                await blobClient.UploadAsync(memoryStream, overwrite: true);
            }
        }
    }

}

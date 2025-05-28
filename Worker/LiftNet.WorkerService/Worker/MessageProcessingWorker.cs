using LiftNet.Domain.Interfaces;
using LiftNet.ServiceBus.Interfaces;
using LiftNet.WorkerService.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.WorkerService.Worker
{
    public class MessageProcessingWorker : BackgroundService, IAsyncDisposable
    {
        private readonly ILiftLogger<MessageProcessingWorker> _logger;
        private readonly IEventConsumer _eventConsumer;

        public MessageProcessingWorker(IEventConsumer eventConsumer, ILiftLogger<MessageProcessingWorker> logger)
        {
            _eventConsumer = eventConsumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cts)
        {
            _logger.Info("RabbitMQ Consumer Service is starting...");

            var queueConfig = WorkerUtil.GetQueueConfig();
            if (queueConfig?.QueueNames == null || queueConfig.QueueNames.Count == 0)
            {
                _logger.Warn("No queues to consume.");
                return;
            }

            var tasks = queueConfig.QueueNames
                .Select(queueName => ConsumeQueueAsync(queueName, cts))
                .ToList();

            await Task.WhenAll(tasks);

            _logger.Info("RabbitMQ Consumer Service is stopping...");
        }

        private async Task ConsumeQueueAsync(string queueName, CancellationToken cts)
        {
            try
            {
                _logger.Info($"Started consuming queue: {queueName}");

                await _eventConsumer.StartConsumingAsync(queueName, cts);

                _logger.Info($"Stopped consuming queue: {queueName}");
            }
            catch (OperationCanceledException)
            {
                _logger.Info($"Consumption cancelled for queue: {queueName}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error while consuming queue {queueName}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _eventConsumer.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }

}

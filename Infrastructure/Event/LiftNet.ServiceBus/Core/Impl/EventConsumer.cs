using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LiftNet.ServiceBus.Factory;
using LiftNet.Domain.Interfaces;

namespace LiftNet.ServiceBus.Core.Impl
{
    public class EventConsumer : IEventConsumer, IAsyncDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqCredentials _credentials;

        private ILiftLogger<EventConsumer> _logger => _serviceProvider.GetRequiredService<ILiftLogger<EventConsumer>>();

        public EventConsumer(RabbitMqCredentials credentials, IServiceProvider serviceProvider)
        {
            _credentials = credentials;
            _serviceProvider = serviceProvider;
            InitializeConnectionAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeConnectionAsync()
        {
            var factory = new ConnectionFactory
            {
                //HostName = _credentials.Hostname,
                UserName = _credentials.Username,
                Password = _credentials.Password,
                Uri = new Uri(_credentials.Url),
                //Port = _credentials.Port,
                AutomaticRecoveryEnabled = false,
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        private async Task ReconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
            await InitializeConnectionAsync();
        }

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                await ReconnectAsync();
            }
        }

        public async Task StartConsumingAsync(string queueName, CancellationToken cts)
        {
            await EnsureConnectionAsync();

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                if (cts.IsCancellationRequested)
                {
                    _logger.Info("Cancellation requested. Ignoring incoming message.");
                    return;
                }

                _logger.Info("Received message!");
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (ea.BasicProperties.Headers != null &&
                        ea.BasicProperties.Headers.TryGetValue("x-redelivery-count", out var countObj) &&
                        int.TryParse(countObj?.ToString(), out var count))
                    {
                        if (count >= 10)
                        {
                            _logger.Warn($"Message redelivered {count} times. Dropping message.");
                            await _channel.BasicRejectAsync(ea.DeliveryTag, requeue: false);
                            return;
                        }
                    }

                    var eventMessage = JsonConvert.DeserializeObject<EventMessage>(message);
                    await HandleEvent(eventMessage!);

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error processing message: {ex.Message}");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            var consumerTag = await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            _logger.Info($"Consumer started for queue: {queueName}");

            // Block until cancellation is requested
            try
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(500, cts); // polling interval
                }
                _logger.Warn($"Cancellation requested for queue: {queueName}");
            }
            catch (TaskCanceledException)
            {
                _logger.Info($"Cancellation token triggered for queue: {queueName}");
            }

            // Cancel the consumer when token is triggered
            await _channel.BasicCancelAsync(consumerTag);
            _logger.Info($"Consumer stopped for queue: {queueName}");
        }

        private async Task HandleEvent(EventMessage eventMessage)
        {
            var eventType = eventMessage.Type;
            _logger.Info($"Processing event: {eventType}");
            var factory = new EventProcessorFactory(eventMessage, _serviceProvider);

            await factory.GetProcessor().Process();
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }
    }
}

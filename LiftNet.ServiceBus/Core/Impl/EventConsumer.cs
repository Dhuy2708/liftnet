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
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly IServiceProvider _serviceProvider;

        private ILiftLogger<EventConsumer> _logger => _serviceProvider.GetRequiredService<ILiftLogger<EventConsumer>>();

        public EventConsumer(IConnection connection, IServiceProvider serviceProvider)
        {
            _connection = connection;
            _serviceProvider = serviceProvider;
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult(); // Khởi tạo đồng bộ
        }

        public async Task StartConsumingAsync(string queueName)
        {
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
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

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            _logger.Info($"Consumer started for queue: {queueName}");
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

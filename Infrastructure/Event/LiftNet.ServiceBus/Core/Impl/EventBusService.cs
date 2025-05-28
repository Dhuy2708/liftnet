using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Interfaces;
using LiftNet.Domain.Interfaces;

namespace LiftNet.ServiceBus.Core.Impl
{
    public class EventBusService : IEventBusService
    {
        private IConnection connection;
        private IChannel channel;
        private readonly ILiftLogger<EventBusService> _logger;
        private readonly RabbitMqCredentials _credentials;

        public EventBusService(RabbitMqCredentials credentials, ILiftLogger<EventBusService> logger)
        {
            _credentials = credentials;
            _logger = logger;
        }

        public async Task InitializeAsync()
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

            connection = await factory.CreateConnectionAsync();
            channel = await connection.CreateChannelAsync();
        }

        private async Task ReconnectAsync()
        {
            if (connection != null)
            {
                await connection.CloseAsync(); // Close old connection if needed
            }
            await InitializeAsync();
        }

        private async Task EnsureConnectionAsync()
        {
            if (connection == null || !connection.IsOpen)
            {
                await ReconnectAsync();
            }
        }

        public async Task PublishAsync(EventMessage message, string queueName)
        {
            await EnsureConnectionAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.Info("Message has been published");
        }

        public async ValueTask DisposeAsync()
        {
            if (channel != null)
            {
                await channel.DisposeAsync();
            }

            connection?.CloseAsync(); // still sync
        }
    }
}

using LiftNet.ServiceBus.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Interfaces
{
    public interface IEventBusService : IAsyncDisposable
    {
        Task PublishAsync(EventMessage message, string queueName);
    }
}

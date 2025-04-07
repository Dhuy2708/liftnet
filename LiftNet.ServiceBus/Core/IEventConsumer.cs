using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Interfaces
{
    public interface IEventConsumer : IAsyncDisposable
    {
        Task StartConsumingAsync(string queueName, CancellationToken cts);
    }
}

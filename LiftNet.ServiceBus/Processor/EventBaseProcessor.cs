using LiftNet.Domain.Interfaces;
using LiftNet.ServiceBus.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Processor
{
    public abstract class EventBaseProcessor
    {
        protected readonly EventMessage _eventMessage;
        protected IServiceProvider _serviceProvider;
        private ILiftLogger<EventBaseProcessor> logger => _serviceProvider.GetRequiredService<ILiftLogger<EventBaseProcessor>>();
        public EventBaseProcessor(EventMessage eventMessage, IServiceProvider serviceProvider)
        {
            _eventMessage = eventMessage;
            _serviceProvider = serviceProvider;
        }
        public async Task Process()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    await HandleAsync(scope);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        protected abstract Task HandleAsync(IServiceScope scope);
    }
}

using LiftNet.Contract.Enums.Feed;
using LiftNet.Domain.Interfaces;
using LiftNet.ServiceBus.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Processor
{
    public class ReactFeedProcessor : EventBaseProcessor
    {
        private readonly ReactType _type;
        private ILiftLogger<ReactFeedProcessor> logger => _serviceProvider.GetRequiredService<ILiftLogger<ReactFeedProcessor>>();

        public ReactFeedProcessor(EventMessage eventMessage, IServiceProvider serviceProvider, ReactType type)
            : base(eventMessage, serviceProvider)
        {
            _type = type;
        }

        protected override Task HandleAsync(IServiceScope scope)
        {
            throw new NotImplementedException();
        }
    }
}

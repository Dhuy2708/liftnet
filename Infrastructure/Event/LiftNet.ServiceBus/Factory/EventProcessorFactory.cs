using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Feed;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Factory
{
    public class EventProcessorFactory
    {
        private readonly EventMessage _eventMessage;
        private readonly IServiceProvider _serviceProvider;
        public EventProcessorFactory(EventMessage eventMessage, IServiceProvider serviceProvider)
        {
            _eventMessage = eventMessage;
            _serviceProvider = serviceProvider;
        }

        public EventBaseProcessor GetProcessor()
        {
            switch (_eventMessage.Type)
            {
                case EventType.LikeFeed:
                    return new ReactFeedProcessor(_eventMessage, _serviceProvider, ReactType.Like);
                case EventType.UnLikeFeed:
                    return new ReactFeedProcessor(_eventMessage, _serviceProvider, ReactType.UnLike);
                case EventType.Noti:
                    return new NotiEventProcessor(_eventMessage, _serviceProvider);
                default:
                    throw new NotSupportedException($"havent supported this processor, event: {_eventMessage.ToString()}");
            }
        }
    }
}

using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Schedules;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Schedules.Queries.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Schedules.Queries
{
    public class ListEventsHandler : IRequestHandler<ListEventQueryRequest, LiftNetRes<ScheduleView>>
    {
        private readonly ILiftLogger<ListEventsHandler> _logger;
        private readonly IEventIndexService _eventIndexService;

        public ListEventsHandler(ILiftLogger<ListEventsHandler> logger, IEventIndexService eventIndexService)
        {
            _logger = logger;
            _eventIndexService = eventIndexService;
        }

        public Task<LiftNetRes<ScheduleView>> Handle(ListEventQueryRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

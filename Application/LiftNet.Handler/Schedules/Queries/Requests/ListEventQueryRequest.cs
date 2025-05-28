using LiftNet.Contract.Views.Schedules;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Schedules.Queries.Requests
{
    public class ListEventQueryRequest : IRequest<LiftNetRes<EventView>>
    {
        public string UserId
        {
            get; set;
        }

        public DateOnly StartDate
        {
            get; set;
        }

        public DateOnly EndDate
        {
            get; set;
        }

        public string? EventSearch
        {
            get; set;
        }

        public List<string>? UserIds
        {
            get; set;
        }
    }
}

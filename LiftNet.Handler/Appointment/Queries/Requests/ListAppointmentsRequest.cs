using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointment.Queries.Requests
{
    public class ListAppointmentsRequest : IRequest<PaginatedLiftNetRes<AppointmentDto>>
    {
        public string UserId
        {
            get; set;
        }
        public QueryCondition Conditions
        {
            get; set;
        }
    }
}

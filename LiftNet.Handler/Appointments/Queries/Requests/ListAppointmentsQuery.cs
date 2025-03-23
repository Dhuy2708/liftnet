using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Queries.Requests
{
    public class ListAppointmentsQuery : IRequest<PaginatedLiftNetRes<AppointmentOverview>>
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

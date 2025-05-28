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
    public class GetAppointmentQuery : IRequest<LiftNetRes<AppointmentDetailView>>
    {
        public string Id
        {
            get; set;
        }
        public string UserId
        {
            get; set;
        }
    }
}

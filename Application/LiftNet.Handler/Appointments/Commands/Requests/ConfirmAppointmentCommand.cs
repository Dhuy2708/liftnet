using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands.Requests
{
    public class ConfirmAppointmentCommand : IRequest<LiftNetRes>
    {
        public string CallerId
        {
            get; set;
        }

        public int ConfirmRequestId
        {
            get; set;
        }
    }
}

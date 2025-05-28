using LiftNet.Contract.Enums.Appointment;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands.Requests
{
    public class AppointmentActionCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public string AppointmentId
        {
            get; set;
        }
        public AppointmentActionRequestType Action
        {
            get; set;
        }
    }
}

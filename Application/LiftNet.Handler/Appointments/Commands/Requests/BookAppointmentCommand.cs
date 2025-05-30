using LiftNet.Contract.Dtos;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands.Requests
{
    public class BookAppointmentCommand : IRequest<LiftNetRes>
    {
        public AppointmentDto Appointment
        {
            get; set;
        }

        public string CallerId
        {
            get; set;
        }
    }
}

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
    public class EditAppointmentCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public string AppointmentId
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public DateTime? StartTime
        {
            get; set;
        }
        public DateTime? EndTime
        {
            get; set;
        }
        public RepeatingType RepeatingType
        {
            get; set;
        }
        public List<string> ParticipantIds
        {
            get; set;
        }
    }
}

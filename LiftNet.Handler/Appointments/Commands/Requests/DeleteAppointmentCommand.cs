using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Appointments.Commands.Requests
{
    public class DeleteAppointmentCommand : IRequest<LiftNetRes>
    {
        public string AppointmentId { get; set; }
        public string UserId { get; set; }
    }
} 
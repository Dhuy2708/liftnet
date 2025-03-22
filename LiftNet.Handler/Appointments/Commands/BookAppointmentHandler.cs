using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Utility.Mappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class BookAppointmentHandler : IRequestHandler<BookAppointmentCommand, LiftNetRes>
    {
        private readonly IAppointmentRepo _appointmentRepo;

        public BookAppointmentHandler(IAppointmentRepo appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<LiftNetRes> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            request.Appointment.Status = Contract.Enums.AppointmentStatus.Pending;
            var entity = request.Appointment.ToEntity();
            var result = await _appointmentRepo.Create(entity);
            if (result > 0)
            {
                return LiftNetRes.SuccessResponse($"Create appointment {request.Appointment.Name} successfully");
            }
            return LiftNetRes.ErrorResponse($"Create appointment {request.Appointment.Name} failed");
        }
    }
}

using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Queries.Requests;
using LiftNet.Ioc;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Queries
{
    public class GetAppointmentHandler : IRequestHandler<GetAppointmentRequest, LiftNetRes<AppointmentDetailView>>
    {
        private readonly IAppointmentRepo _appointmentRepo;

        public GetAppointmentHandler(IAppointmentRepo appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<LiftNetRes<AppointmentDetailView>> Handle(GetAppointmentRequest request, CancellationToken cancellationToken)
        {
            var queryable = _appointmentRepo.GetQueryable();
            var appointment = await queryable.FirstOrDefaultAsync(x => x.Id == request.Id && 
                                                       (x.ClientId == request.UserId || x.CoachId == request.UserId));
            if (appointment == null)
            {
                throw new NotFoundException("Appointment not found");
            }
            return LiftNetRes<AppointmentDetailView>.SuccessResponse(appointment.ToDetailView());
        }
    }
}

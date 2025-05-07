using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
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
    public class GetAppointmentHandler : IRequestHandler<GetAppointmentQuery, LiftNetRes<AppointmentDetailView>>
    {
        private readonly ILiftLogger<GetAppointmentHandler> _logger;
        private readonly IAppointmentRepo _appointmentRepo;

        public GetAppointmentHandler(IAppointmentRepo appointmentRepo, ILiftLogger<GetAppointmentHandler> logger)
        {
            _appointmentRepo = appointmentRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes<AppointmentDetailView>> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
        {
            _logger.Info($"get appointment detail, appointmentId: {request.Id}");
            var queryable = _appointmentRepo.GetQueryable();
            queryable = queryable.Include(x => x.Booker)
                                 .Include(x => x.Participants)
                                 .ThenInclude(x => x.User);
            var appointment = await queryable.FirstOrDefaultAsync(x => x.Id == request.Id && 
                                                       (x.Participants.Select(p => p.UserId).Contains(request.UserId)));
            if (appointment == null)
            {
                throw new NotFoundException("Appointment not found");
            }
            return LiftNetRes<AppointmentDetailView>.SuccessResponse(appointment.ToDetailView(request.UserId.Eq(appointment.BookerId!)));
        }
    }
}

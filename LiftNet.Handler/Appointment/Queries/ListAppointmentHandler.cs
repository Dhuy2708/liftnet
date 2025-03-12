using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointment.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointment.Queries
{
    public class ListAppointmentHandler : IRequestHandler<ListAppointmentsRequest, PaginatedLiftNetRes<AppointmentDto>>
    {
        private readonly IAppointmentRepo _appointmentRepo;

        public ListAppointmentHandler(IAppointmentRepo appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<PaginatedLiftNetRes<AppointmentDto>> Handle(ListAppointmentsRequest request, CancellationToken cancellationToken)
        {
            var queryable = _appointmentRepo.GetQueryable();
            var conditions = request.Conditions;

            var query = queryable.Where(x => x.ClientId.Eq(request.UserId) || x.CoachId.Eq(request.UserId));
            query.Take(conditions.PageSize);
            query.Skip((conditions.PageNumber - 1) * conditions.PageSize);

            var appointments = await query.ToListAsync();
     
            var count = await _appointmentRepo.GetCount();
            var appointmentDtos = appointments.Select(x => x.ToDto()).ToList();
            return PaginatedLiftNetRes<AppointmentDto>.SuccessResponse(appointmentDtos, conditions.PageNumber, conditions.PageSize, count);
        }
    }
}

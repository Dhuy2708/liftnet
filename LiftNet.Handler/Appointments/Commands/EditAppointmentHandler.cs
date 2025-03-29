using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class EditAppointmentHandler : IRequestHandler<EditAppointmentCommand, LiftNetRes>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILiftLogger<EditAppointmentHandler> _logger;

        public EditAppointmentHandler(IUnitOfWork uow, ILiftLogger<EditAppointmentHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(EditAppointmentCommand request, CancellationToken cancellationToken)
        {
            var queryable = _uow.AppointmentRepo.GetQueryable();
            queryable = queryable.Include(x => x.Participants);

            var count = await _uow.UserRepo.GetQueryable()
                                           .CountAsync(x => !x.IsDeleted && 
                                                            request.ParticipantIds.Contains(x.Id));
            if (count < request.ParticipantIds.Count)
            {
                return LiftNetRes.ErrorResponse("Some participants not found");
            }

            var appointment = await queryable.FirstOrDefaultAsync(x => x.Id == request.AppointmentId && 
                                                                       x.BookerId == request.UserId);

            if (appointment == null)
            {
                return LiftNetRes.ErrorResponse("Appointment not found");
            }

            _logger.Info("begin edit appointment");
            if (request.ParticipantIds.IsNotNullOrEmpty())
            {
                if (!request.ParticipantIds.Contains(request.UserId))
                {
                    return LiftNetRes.ErrorResponse("Can't remove yourself");
                }
                var newParticipants = request.ParticipantIds.Select(x => new AppointmentParticipant()
                {
                    AppointmentId = appointment.Id,
                    UserId = x,
                }).ToList();
                appointment.Participants = newParticipants;
            }
            if (request.Name.IsNotNullOrEmpty())
            {
                appointment.Name = request.Name;
            }
            if (request.Description.IsNotNullOrEmpty())
            {
                appointment.Description = request.Description;
            }
            if (request.StartTime != null && !request.StartTime.Equals(DateTime.MinValue))
            {
                appointment.StartTime = request.StartTime.Value;
            }
            if (request.EndTime != null && !request.EndTime.Equals(DateTime.MinValue))
            {
                appointment.EndTime = request.EndTime.Value;
            }
            appointment.Modified = DateTime.UtcNow;
            var result = await _uow.CommitAsync();
            if (result > 0)
            {
                return LiftNetRes.SuccessResponse("Appointment updated successfully");
            }
            return LiftNetRes.ErrorResponse("Failed to update appointment");
        }
    }
}

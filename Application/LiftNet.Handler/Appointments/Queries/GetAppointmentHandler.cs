using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Queries.Requests;
using LiftNet.Ioc;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using LiftNet.Utility.Utils;
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
        private readonly IAppointmentSeenStatusRepo _seenStatusRepo;
        private readonly IAppointmentConfirmationRepo _confirmationRepo;

        public GetAppointmentHandler(ILiftLogger<GetAppointmentHandler> logger, 
                                     IAppointmentRepo appointmentRepo,
                                     IAppointmentSeenStatusRepo seenStatusRepo, 
                                     IAppointmentConfirmationRepo confirmationRepo)
        {
            _logger = logger;
            _appointmentRepo = appointmentRepo;
            _seenStatusRepo = seenStatusRepo;
            _confirmationRepo = confirmationRepo;
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
            var status = GetCurrentUserStatusFromAppointment(appointment, request.UserId);

            await ResetSeenStatus(appointment, request.UserId);

            var detail = appointment.ToDetailView(request.UserId.Eq(appointment.BookerId!), status: status);
            var confirmationReq = await _confirmationRepo.GetQueryable()
                                                         .FirstOrDefaultAsync(x => x.AppointmentId == appointment.Id);
            if (confirmationReq != null)
            {
                detail.ConfirmationRequest = new AppointmentConfirmationRequestView
                {
                    Id = confirmationReq.Id,
                    Img = confirmationReq.Img,
                    Content = confirmationReq.Content,
                    Status = (AppointmentConfirmationStatus)confirmationReq.Status,
                    CreatedAt = confirmationReq.CreatedAt.ToOffSet(),
                    ModifiedAt = confirmationReq.ModifiedAt.ToOffSet(),
                    ExpiresdAt = confirmationReq.ExpiredAt.ToOffSet(),
                };
            }
          
            return LiftNetRes<AppointmentDetailView>.SuccessResponse(detail);
        }

        private AppointmentParticipantStatus GetCurrentUserStatusFromAppointment(Appointment appointment, string userId)
        {
            var participant = appointment?.Participants?.FirstOrDefault(x => x.UserId == userId);
            if (participant != null)
            {
                return (AppointmentParticipantStatus)participant.Status;
            }
            return AppointmentParticipantStatus.None;
        }

        private async Task ResetSeenStatus(Appointment appointment, string callerId)
        {
            var queryable = _seenStatusRepo.GetQueryable();

            var seenStatus = await queryable.FirstOrDefaultAsync(x => x.AppointmentId == appointment.Id &&
                                                                  x.UserId == callerId);

            if (seenStatus != null)
            {
                seenStatus.NotiCount = 0;
                seenStatus.LastSeen = DateTime.UtcNow;
                await _seenStatusRepo.Update(seenStatus);
            }
            else
            {
                var newSeenStatus = new AppointmentSeenStatus
                {
                    UserId = callerId,
                    AppointmentId = appointment.Id,
                    NotiCount = 0,
                    LastSeen = DateTime.UtcNow,
                    LastUpdate = appointment.Modified
                };
                await _seenStatusRepo.Create(newSeenStatus);
            }
        }
    }
}

using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
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
        private readonly IFeedbackRepo _feedbackRepo;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public GetAppointmentHandler(ILiftLogger<GetAppointmentHandler> logger, 
                                     IAppointmentRepo appointmentRepo,
                                     IAppointmentSeenStatusRepo seenStatusRepo, 
                                     IAppointmentConfirmationRepo confirmationRepo,
                                     IFeedbackRepo feedbackRepo,
                                     IUserService userService,
                                     IRoleService roleService)
        {
            _logger = logger;
            _appointmentRepo = appointmentRepo;
            _seenStatusRepo = seenStatusRepo;
            _confirmationRepo = confirmationRepo;
            _feedbackRepo = feedbackRepo;
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<LiftNetRes<AppointmentDetailView>> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
        {
            _logger.Info($"get appointment detail, appointmentId: {request.Id}");
            var queryable = _appointmentRepo.GetQueryable();
            queryable = queryable.Include(x => x.Booker)
                                 .ThenInclude(x => x.UserRoles)
                                 .Include(x => x.Participants)
                                 .ThenInclude(x => x.User)
                                 .ThenInclude(x => x.UserRoles);
            var appointment = await queryable.FirstOrDefaultAsync(x => x.Id == request.Id && 
                                                       (x.Participants.Select(p => p.UserId).Contains(request.UserId)));
            if (appointment == null)
            {
                throw new NotFoundException("Appointment not found");
            }
            var status = GetCurrentUserStatusFromAppointment(appointment, request.UserId);

            await ResetSeenStatus(appointment, request.UserId);

            var roleDict = await _roleService.GetAllRoleDictAsync();
            var detail = appointment.ToDetailView(roleDict, request.UserId.Eq(appointment.BookerId!), status: status);

            await AssignConfirmationRequest(detail);
            await AssignFeedbacks(detail);

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

        private async Task AssignConfirmationRequest(AppointmentDetailView detail)
        {
            var confirmationReq = await _confirmationRepo.GetQueryable()
                                                         .FirstOrDefaultAsync(x => x.AppointmentId == detail.Id);
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
        }

        private async Task AssignFeedbacks(AppointmentDetailView appointment)
        {
            var feedbacks = await _feedbackRepo.GetQueryable()
                                               .Include(x => x.Reviewer)
                                               .Where(x => x.ApppointmentId == appointment.Id)
                                               .OrderByDescending(x => x.CreatedAt)
                                               .ToListAsync();

            if (feedbacks.IsNullOrEmpty())
            {
                return;
            }
            var reviewerIds = feedbacks.Select(x => x.ReviewerId).Distinct().ToList();
            var userRoleMapping = await _userService.GetUserIdRoleDict(reviewerIds);
            var feedbackView = feedbacks.ToViews(userRoleMapping);

            appointment.Feedbacks = feedbackView;
        }
    }
}

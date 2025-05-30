using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Handler.Appointments.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class AppointmentActionHandler : IRequestHandler<AppointmentActionCommand, LiftNetRes>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILiftLogger<AppointmentActionHandler> _logger;

        public AppointmentActionHandler(IUnitOfWork uow, ILiftLogger<AppointmentActionHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(AppointmentActionCommand request, CancellationToken cancellationToken)
        {
            await new AppointmentActionValidator().ValidateAndThrowAsync(request);
            
            _logger.Info("begin to handle appointment action command");
            var appointmentRepo = _uow.AppointmentRepo;
            var participantRepo = _uow.AppointmentParticipantRepo;
            
            var appointment = await appointmentRepo.GetQueryable()
                .Include(x => x.Participants)
                .FirstOrDefaultAsync(x => x.Id == request.AppointmentId);

            if (appointment == null)
            {
                _logger.Error("appointment not found");
                return LiftNetRes.ErrorResponse("Appointment not found");
            }

            var participant = appointment.Participants.FirstOrDefault(p => p.UserId == request.UserId);
            if (participant == null)
            {
                _logger.Error("user is not a participant of this appointment");
                return LiftNetRes.ErrorResponse("You are not a participant of this appointment");
            }

            if (appointment.StartTime < DateTime.UtcNow)
            {
                _logger.Error("appointment has already started");
                return LiftNetRes.ErrorResponse("This appointment is expired");
            }

            if (!await CheckBalance(request.UserId, appointment.Price))
            {
                return LiftNetRes.ErrorResponse("You do not have enough balance to");
            }

            appointment.Modified = DateTime.UtcNow;

            if (request.Action is AppointmentActionRequestType.Accept)
            {
                participant.Status = (int)AppointmentParticipantStatus.Accepted;
            }
            else if (request.Action is AppointmentActionRequestType.Reject)
            {
                if (participant.Status != (int)AppointmentParticipantStatus.Pending)
                {
                    _logger.Error("participant status is not pending");
                    return LiftNetRes.ErrorResponse("Your status is not pending to reject");
                }
                participant.Status = (int)AppointmentParticipantStatus.Rejected;
            }
            else if(request.Action is AppointmentActionRequestType.Cancel)
            {
                if (participant.Status != (int)AppointmentParticipantStatus.Accepted)
                {
                    _logger.Error("participant status is not accepted");
                    return LiftNetRes.ErrorResponse("Your status is not accepted to cancel");
                }
                participant.Status = (int)AppointmentParticipantStatus.Canceled;
            }
            else
            {
                _logger.Error("invalid action");
                return LiftNetRes.ErrorResponse("Invalid action");
            }
            await UpdateSeenStatus(appointment, request.UserId);
            await _uow.CommitAsync();
            _logger.Info("appointment action handled successfully");
            return LiftNetRes.SuccessResponse("Appointment action handled successfully");
        }

        private async Task UpdateSeenStatus(Appointment appointment, string callerId)
        {
            var queryable = _uow.AppointmentSeenStatusRepo.GetQueryable();
            var otherParticipantIds = appointment.Participants.Where(x => !x.UserId.Eq(callerId))
                                                              .Select(x => x.UserId)
                                                              .ToList();

            var seenStatuses = await queryable.Where(x => x.AppointmentId == appointment.Id &&
                                                        otherParticipantIds.Contains(x.UserId))
                                            .ToListAsync();

            var existingUserIds = seenStatuses.Select(x => x.UserId).ToList();

            var nonExistUserIds = otherParticipantIds.Except(existingUserIds).ToList();

            if (nonExistUserIds.Any())
            {
                var newSeenStatuses = nonExistUserIds.Select(userId => new AppointmentSeenStatus
                {
                    AppointmentId = appointment.Id,
                    UserId = userId,
                    NotiCount = 1,
                    LastSeen = null,
                    LastUpdate = DateTime.UtcNow
                }).ToList();
                await _uow.AppointmentSeenStatusRepo.CreateRange(newSeenStatuses);
            }
            else
            {
                foreach (var seenStatus in seenStatuses)
                {
                    seenStatus.NotiCount += 1;
                    seenStatus.LastUpdate = DateTime.UtcNow;
                }
                await _uow.AppointmentSeenStatusRepo.UpdateRange(seenStatuses);
            }
        }

        private async Task<bool> CheckBalance(string userId, int appointmentPrice)
        {
            var balance = (await _uow.WalletRepo.GetQueryable()
                                         .FirstOrDefaultAsync(x => x.UserId == userId))?.Balance ?? 0;
            return balance >= appointmentPrice;
        }
    }
}

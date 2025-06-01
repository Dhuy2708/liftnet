using LiftNet.Contract.Dtos.Transaction;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Payment;
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
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class AppointmentActionHandler : IRequestHandler<AppointmentActionCommand, LiftNetRes>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILiftLogger<AppointmentActionHandler> _logger;
        private Wallet Wallet;

        public AppointmentActionHandler(IUnitOfWork uow, ILiftLogger<AppointmentActionHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(AppointmentActionCommand request, CancellationToken cancellationToken)
        {
            await new AppointmentActionValidator().ValidateAndThrowAsync(request);

            try
            {
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
                if (appointment.BookerId.IsNullOrEmpty())
                {
                    return LiftNetRes.ErrorResponse("Cant find booker of this appointment");
                }
                var participant = appointment.Participants.FirstOrDefault(p => p.UserId == request.UserId);
                if (participant == null)
                {
                    _logger.Error("user is not a participant of this appointment");
                    return LiftNetRes.ErrorResponse("You are not a participant of this appointment");
                }
                if (appointment.BookerId.Eq(request.UserId))
                {
                    return LiftNetRes.ErrorResponse("Bookers cannot response to their appointment");
                }
                if (appointment.StartTime < DateTime.UtcNow)
                {
                    _logger.Error("appointment has already started");
                    return LiftNetRes.ErrorResponse("This appointment is expired");
                }

                await InitWallet(request.UserId);
                appointment.Modified = DateTime.UtcNow;

                if (request.Action is AppointmentActionRequestType.Accept)
                {

                    if (!CheckBalance(request.UserId, appointment.Price))
                    {
                        return LiftNetRes.ErrorResponse("You do not have enough balance to accept this appointment");
                    }
                    await HandleTransaction(appointment, appointment.BookerId!, request.UserId, request.Action);
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
                else if (request.Action is AppointmentActionRequestType.Cancel)
                {
                    if (participant.Status != (int)AppointmentParticipantStatus.Accepted)
                    {
                        _logger.Error("participant status is not accepted");
                        return LiftNetRes.ErrorResponse("Your status is not accepted to cancel");
                    }
                    await HandleTransaction(appointment, appointment.BookerId!, request.UserId, request.Action);
                    participant.Status = (int)AppointmentParticipantStatus.Canceled;
                }
                else
                {
                    _logger.Error("invalid action");
                    return LiftNetRes.ErrorResponse("Invalid action");
                }
                AsssignAllAcceptedStatus(appointment);
                await UpdateSeenStatus(appointment, request.UserId);
                await _uow.CommitAsync();
                _logger.Info("appointment action handled successfully");
                return LiftNetRes.SuccessResponse("Appointment action handled successfully");
            }
            catch (Exception e)
            {
                await _uow.RollbackAsync();
                _logger.Error(e, "Error handling appointment action command");
                return LiftNetRes.ErrorResponse("An error occurred while processing your request.");
            }
        }

        private void AsssignAllAcceptedStatus(Appointment appointment)
        {
            if (appointment.Participants.All(x => x.Status == (int)AppointmentParticipantStatus.Accepted))
            {
                appointment.AllAccepted = true;
            }
            else
            {
                appointment.AllAccepted = false;
            }
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

        private async Task InitWallet(string userId)
        {
            var wallet = await _uow.WalletRepo.GetQueryable()
                                         .FirstOrDefaultAsync(x => x.UserId == userId);
            if (wallet == null)
            {
                var newWallet = new Wallet()
                {
                    UserId = userId,
                    Balance = 0
                };
                await _uow.WalletRepo.Create(newWallet);
                await _uow.CommitAsync();
                throw new ApplicationException();
            }
            this.Wallet = wallet;
        }

        private bool CheckBalance(string userId, int appointmentPrice)
        {
            return Wallet.Balance >= appointmentPrice;
        }

        private async Task HandleTransaction(Appointment appointment, string bookerId, string callerId, AppointmentActionRequestType type)
        {
            if (appointment.Price == 0)
            {
                return;
            }
            var transferAmount = 0;
            if (type is AppointmentActionRequestType.Accept)
            {
                transferAmount = -appointment.Price;
            }
            else if (type is AppointmentActionRequestType.Cancel)
            {
                transferAmount = appointment.Price;
            }
            Wallet.Balance += transferAmount;

            var transaction = new LiftNetTransaction()
            {
                UserId = callerId,
                Amount = transferAmount,
                Description = "Payment for appointment " + appointment.Name,
                Status = (int)TransactionStatus.Success,
                FromUserId = callerId,
                ToUserId = null, // go to the fund hold
                CreatedAt = DateTime.UtcNow,
            };
            await _uow.WalletRepo.Update(Wallet);
            await _uow.LiftNetTransactionRepo.Create(transaction);
        }
    }
}

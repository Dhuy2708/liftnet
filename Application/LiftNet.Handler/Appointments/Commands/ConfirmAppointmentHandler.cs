using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Payment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class ConfirmAppointmentHandler : IRequestHandler<ConfirmAppointmentCommand, LiftNetRes>
    {
        private readonly ILiftLogger<ConfirmAppointmentHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IAppointmentService _appointmentService;

        public ConfirmAppointmentHandler(ILiftLogger<ConfirmAppointmentHandler> logger,
                                         IUnitOfWork uow,
                                         IAppointmentService appointmentService)
        {
            _logger = logger;
            _uow = uow;
            _appointmentService = appointmentService;
        }

        public async Task<LiftNetRes> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var requestId = request.ConfirmRequestId;
                var callerId = request.CallerId;
              

                var confirmation = await _uow.AppointmentConfirmationRepo.GetQueryable()
                                                .Include(x => x.Appointment)
                                                .ThenInclude(x => x.Participants)
                                                .FirstOrDefaultAsync(x => x.Id == requestId);

                if (confirmation == null)
                {
                    throw new BadRequestException(["Confirmation request with ID {requestId} does not exist."]);
                }

                if (!CheckPermission(confirmation.Appointment, callerId))
                {
                    return LiftNetRes.ErrorResponse("You do not have permission to confirm this appointment.");
                }

                await _uow.BeginTransactionAsync();

                if (confirmation.Status != (int)AppointmentConfirmationStatus.Requested)
                {
                    return LiftNetRes.ErrorResponse("This confirmation request is not in a valid state to be confirmed.");
                }

                confirmation.Status = (int)AppointmentConfirmationStatus.Confirmed;
                confirmation.ModifiedAt = DateTime.UtcNow;

                await _uow.AppointmentConfirmationRepo.Update(confirmation);
                await HandleTransaction(confirmation.Appointment);
                await UpdateCoachExtension(confirmation.Appointment.BookerId!);
                var result = await _uow.CommitAsync();

                if (result > 0)
                {
                    await _appointmentService.PingAppointmentNotiCount(confirmation.Appointment.Id, callerId);
                }

                return LiftNetRes.SuccessResponse("Appointment confirmed successfully.");
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Error in ConfirmAppointmentHandler, confirmationRequestId: {request.ConfirmRequestId}");
                return LiftNetRes.ErrorResponse("An error occurred while confirming the appointment.");
            }
        }

        private bool CheckPermission(Appointment appointment, string callerId)
        {
            if (appointment.BookerId == callerId)
            {
                return false;
            }
            if (AppointmentUtil.GetAppointmentStatus(appointment) != AppointmentStatus.Finished)
            {
                return false;
            }
            if (!appointment.Participants.Select(x => x.UserId).Contains(callerId))
            {
                return false;
            }

            return true;
        }

        private async Task HandleTransaction(Appointment appointment)
        {
            var price = appointment.Price;

            if (price <= 0)
            {
                return;
            }

            var transaction = await _uow.LiftNetTransactionRepo.GetQueryable()
                                        .FirstOrDefaultAsync(x => x.AppointmentId == appointment.Id);
            if (transaction == null ||
                transaction.ToUser != null ||
                transaction.Status != (int)LiftNetTransactionStatus.Hold)
            {
                return;
            }

            transaction.ToUserId = appointment.BookerId;
            transaction.Status = (int)LiftNetTransactionStatus.Success;
            transaction.LastUpdate = DateTime.UtcNow;

            // wallet
            var wallet = await _uow.WalletRepo.GetQueryable()
                                   .FirstOrDefaultAsync(x => x.UserId == appointment.BookerId);
            if (wallet == null)
            {
                var newWallet = new Wallet
                {
                    UserId = appointment.BookerId!,
                    Balance = transaction.Amount,
                    LastUpdate = DateTime.UtcNow,
                };
                await _uow.WalletRepo.Create(newWallet);
            }
            else
            {
                wallet.Balance += Math.Abs(transaction.Amount);
                wallet.LastUpdate = transaction.LastUpdate;
                await _uow.WalletRepo.Update(wallet);
            }
            await _uow.LiftNetTransactionRepo.Update(transaction);
        }

        private async Task UpdateCoachExtension(string coachId)
        {
            var coachExtension = await _uow.CoachExtensionRepo.GetQueryable()
                                            .FirstOrDefaultAsync(x => x.CoachId == coachId);
            if (coachExtension == null)
            {
                coachExtension = new CoachExtension
                {
                    CoachId = coachId,
                    SessionTrained = 1,
                    ReviewCount = 0,
                    Star = 0,
                };
                await _uow.CoachExtensionRepo.Create(coachExtension);
            }
            else
            {
                coachExtension.SessionTrained += 1;
                await _uow.CoachExtensionRepo.Update(coachExtension);
            }
        }
    }
}

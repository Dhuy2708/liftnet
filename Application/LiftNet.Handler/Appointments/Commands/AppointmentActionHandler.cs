using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Handler.Appointments.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
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

            appointment.Modified = DateTime.UtcNow;

            if (request.Action is AppointmentActionRequestType.Accept)
            {
                participant.Status = (int)AppointmentStatus.Accepted;
            }
            else if (request.Action is AppointmentActionRequestType.Reject)
            {
                if (participant.Status != (int)AppointmentStatus.Pending)
                {
                    _logger.Error("participant status is not pending");
                    return LiftNetRes.ErrorResponse("Your status is not pending to reject");
                }
                participant.Status = (int)AppointmentStatus.Rejected;
            }
            else if(request.Action is AppointmentActionRequestType.Cancel)
            {
                if (participant.Status != (int)AppointmentStatus.Accepted)
                {
                    _logger.Error("participant status is not accepted");
                    return LiftNetRes.ErrorResponse("Your status is not accepted to cancel");
                }
                participant.Status = (int)AppointmentStatus.Canceled;
            }
            else
            {
                _logger.Error("invalid action");
                return LiftNetRes.ErrorResponse("Invalid action");
            }

            await _uow.CommitAsync();
            _logger.Info("appointment action handled successfully");
            return LiftNetRes.SuccessResponse("Appointment action handled successfully");
        }
    }
}

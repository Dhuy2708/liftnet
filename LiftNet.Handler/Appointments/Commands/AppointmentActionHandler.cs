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
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            appointmentRepo.AutoSave = true;
            var appointment = await appointmentRepo.GetQueryable()
                                                   .FirstOrDefaultAsync(x => x.Id == request.AppointmentId &&
                                                                            x.Participants.Select(x => x.UserId).Contains(request.UserId));
            if (appointment == null)
            {
                _logger.Error("appointment not found");
                return LiftNetRes.ErrorResponse("Appointment not found");
            }
            appointment.Modified = DateTime.UtcNow;
            if (request.Action is AppointmentActionRequestType.Accept)
            {
                if (appointment.BookerId == request.UserId)
                {
                    _logger.Error("booker cannot accept appointment");
                    return LiftNetRes.ErrorResponse("Booker cannot accept appointment");
                }
                if (appointment.Status != (int)AppointmentStatus.Pending)
                {
                    _logger.Error("appointment status is not pending");
                    return LiftNetRes.ErrorResponse("Appointment status is not pending to accept");
                }
                appointment.Status = (int)AppointmentStatus.Accepted;
                await appointmentRepo.SaveChangesAsync();
            }
            else if (request.Action is AppointmentActionRequestType.Reject)
            {
                if (appointment.Status != (int)AppointmentStatus.Pending)
                {
                    _logger.Error("appointment status is not pending");
                    return LiftNetRes.ErrorResponse("Appointment status is not pending to reject");
                }
                appointment.Status = (int)AppointmentStatus.Rejected;
                await appointmentRepo.SaveChangesAsync();
            }
            else if(request.Action is AppointmentActionRequestType.Cancel)
            {
                if (appointment.Status != (int)AppointmentStatus.Accepted)
                {
                    _logger.Error("appointment status is not accepted");
                    return LiftNetRes.ErrorResponse("Appointment status is not accepted to cancel");
                }
                appointment.Status = (int)AppointmentStatus.Canceled;
                await appointmentRepo.SaveChangesAsync();
            }
            else
            {
                _logger.Error("invalid action");
                return LiftNetRes.ErrorResponse("Invalid action");
            }
            _logger.Info("appointment action handled successfully");
            return LiftNetRes.SuccessResponse("Appointment action handled successfully");
        }
    }
}

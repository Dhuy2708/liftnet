using FluentValidation;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Handler.Appointments.Commands.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands.Validators
{
    public class AppointmentActionValidator : AbstractValidator<AppointmentActionCommand>
    {
        public AppointmentActionValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("ProfileId is required.");

            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("AppointmentId is required.");

            RuleFor(x => x.Action)
                .IsInEnum().WithMessage("Invalid action.")
                .Must(action => action == AppointmentActionRequestType.Accept
                             || action == AppointmentActionRequestType.Reject
                             || action == AppointmentActionRequestType.Cancel)
                .WithMessage("Action must be Accept, Reject, or Cancel.");
        }
    }
}

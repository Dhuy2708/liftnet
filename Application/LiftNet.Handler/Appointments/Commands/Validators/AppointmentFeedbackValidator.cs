using FluentValidation;
using LiftNet.Handler.Appointments.Commands.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands.Validators
{
    public class AppointmentFeedbackValidator : AbstractValidator<AppointmentFeedbackCommand>
    {
        public AppointmentFeedbackValidator()
        {
            RuleFor(x => x.Star)
                        .InclusiveBetween(1, 5)
                        .WithMessage("Star must be between 0 and 5.");
        }
    }
}

using FluentValidation;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Handler.Auths.Commands.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auths.Commands.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Role)
                 .NotEmpty().WithMessage("Role is required.")
                 .IsInEnum().WithMessage("Invalid role.")
                 .Must(role => role != LiftNetRoleEnum.Admin)
                 .WithMessage("Admin role is not allowed.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must be at most 100 characters long.")
                .Matches(@"^(?=.*\d)(?=.*[a-z]).{6,}$")
                .WithMessage("Password must contain at least one digit and one lowercase letter.");

            RuleFor(x => x.Age)
                .NotEmpty().WithMessage("Age is required.")
                .InclusiveBetween(1, 120).WithMessage("Age must be between 1 and 120.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender.");
        }
    }
}

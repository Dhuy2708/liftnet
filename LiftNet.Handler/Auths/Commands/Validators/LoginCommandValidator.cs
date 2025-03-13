using FluentValidation;
using LiftNet.Domain.Entities;
using LiftNet.Handler.Auths.Commands.Requests;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auths.Commands.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private readonly UserManager<User> _userManager;
        public LoginCommandValidator(UserManager<User> userManager)
        {
            _userManager = userManager;
            RuleFor(x => x.Username).NotNull().NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.Username).MustAsync(async (username, cancellation) =>
            {
                var user = await _userManager.FindByNameAsync(username);
                return user != null;
            }).WithMessage("User not found.");
        }
    }
}

using FluentValidation;
using LiftNet.Command.Core.Auth.Requests;
using LiftNet.Command.Core.Auth.Validators;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Response;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Command.Core.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LiftNetRes>
    {
        private UserManager<User> _userManager;
        public RegisterCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<LiftNetRes> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await new RegisterCommandValidator(_userManager).ValidateAndThrowAsync();
        }
    }
}

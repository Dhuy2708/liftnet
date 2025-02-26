using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auth.Commands.Requests;
using LiftNet.Handler.Auth.Commands.Validators;
using LiftNet.Ioc;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auth.Commands
{
    public class LoginHandler : IRequestHandler<LoginCommand, LiftNetRes<string>>, IDependency
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;
        public LoginHandler(UserManager<User> userManager, IAuthRepo authRepo)
        {
            _userManager = userManager;
            _authRepo = authRepo;
        }

        public async Task<LiftNetRes<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            await new LoginCommandValidator(_userManager).ValidateAndThrowAsync(request);

            var loginModel = new LoginModel()
            {
                Username = request.Username,
                Password = request.Password
            };

            var token = await _authRepo.LogInAsync(loginModel);
            return LiftNetRes<string>.SuccessResponse(token);
        }
    }
}

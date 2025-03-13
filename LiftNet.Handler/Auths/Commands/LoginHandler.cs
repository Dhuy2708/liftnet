using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using LiftNet.Handler.Auths.Commands.Validators;
using LiftNet.Ioc;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LiftNet.Handler.Auths.Commands
{
    public class LoginHandler : IRequestHandler<LoginCommand, LiftNetRes<string>>, IDependency
    {
        private readonly ILiftLogger<LoginHandler> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;
        public LoginHandler(UserManager<User> userManager, IAuthRepo authRepo, ILiftLogger<LoginHandler> logger)
        {
            _userManager = userManager;
            _authRepo = authRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            await new LoginCommandValidator(_userManager).ValidateAndThrowAsync(request);

            var loginModel = new LoginModel()
            {
                Username = request.Username,
                Password = request.Password
            };

            _logger.LogInformation($"attempt to login, username: {request.Username}");
            var token = await _authRepo.LogInAsync(loginModel);
            return LiftNetRes<string>.SuccessResponse(token);
        }
    }
}

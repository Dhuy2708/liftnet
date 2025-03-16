using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.IRepos;
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
    public class RegisterHandler : IRequestHandler<RegisterCommand, LiftNetRes>, IDependency
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;
        private readonly ILiftLogger<RegisterHandler> _logger;

        public RegisterHandler(UserManager<User> userManager, IAuthRepo authRepo, ILiftLogger<RegisterHandler> logger)
        {
            _userManager = userManager;
            _authRepo = authRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await new RegisterCommandValidator(_userManager).ValidateAndThrowAsync(request);
            var registerModel = new RegisterModel()
            {
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address
            };
            _logger.Info($"attempt to register, username: {request.Username}");
            var result = await _authRepo.RegisterAsync(registerModel);
            if (result.Succeeded)
            {
                _logger.Info("register user successfully");
                return LiftNetRes.SuccessResponse(message: "User registered successfully.");
            }
            _logger.Error("failed to register user");
            return LiftNetRes.ErrorResponse(message: "Failed to register.", errors: result.Errors.Select(x => x.Description).ToList());
        }
    }
}

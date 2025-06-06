using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using LiftNet.Handler.Auths.Commands.Validators;
using LiftNet.Ioc;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace LiftNet.Handler.Auths.Commands
{
    public class LoginHandler : IRequestHandler<LoginCommand, LiftNetRes<TokenInfo>>
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

        public async Task<LiftNetRes<TokenInfo>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            await new LoginCommandValidator(_userManager).ValidateAndThrowAsync(request);

            if (request is AdminLoginCommand)
            {
                var result = await HandleLoginAdmin((AdminLoginCommand)request);
                if (result == null)
                {
                    _logger.Error("failed to login as admin");
                    return LiftNetRes<TokenInfo>.ErrorResponse("Username or password is not correct!");
                }
                return LiftNetRes<TokenInfo>.SuccessResponse(result);
            }

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                throw new NotFoundException("Email or username not exist");
            }

            var loginModel = new LoginModel()
            {
                Username = request.Username,
                Password = request.Password
            };

            _logger.Info($"attempt to login, username: {request.Username}");
            var token = await _authRepo.LogInAsync(loginModel);
            if (token == null)
            {
                _logger.Error("failed to login");
                return LiftNetRes<TokenInfo>.ErrorResponse("Username or password is not correct!");
            }

            _logger.Info("login successfully");
            var tokenInfo = new TokenInfo()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            };
            return LiftNetRes<TokenInfo>.SuccessResponse(tokenInfo);
        }

        private async Task<TokenInfo> HandleLoginAdmin(AdminLoginCommand request)
        {
            var model = new LoginModel 
            { 
                Password = request.Password, 
                Username = request.Username 
            };
            var token = await _authRepo.AdminLoginAsync(model);
            if (token == null)
            {
                _logger.Error("failed to login as admin");
                throw new NotFoundException("Username or password is not correct!");
            }
            var tokenInfo = new TokenInfo()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            };
            return tokenInfo;
        }
    }
}

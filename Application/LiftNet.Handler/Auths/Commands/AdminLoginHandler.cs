using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using LiftNet.Handler.Auths.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auths.Commands
{
    public class AdminLoginHandler : IRequestHandler<AdminLoginCommand, LiftNetRes<TokenInfo>>
    {
        private readonly ILiftLogger<LoginHandler> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;

        public AdminLoginHandler(ILiftLogger<LoginHandler> logger, UserManager<User> userManager, IAuthRepo authRepo)
        {
            _logger = logger;
            _userManager = userManager;
            _authRepo = authRepo;
        }

        public async Task<LiftNetRes<TokenInfo>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        {
            await new LoginCommandValidator(_userManager).ValidateAndThrowAsync(request);

            var result = await HandleLoginAdmin((AdminLoginCommand)request);
            if (result == null)
            {
                _logger.Error("failed to login as admin");
                return LiftNetRes<TokenInfo>.ErrorResponse("Username or password is not correct!");
            }
            return LiftNetRes<TokenInfo>.SuccessResponse(result);
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

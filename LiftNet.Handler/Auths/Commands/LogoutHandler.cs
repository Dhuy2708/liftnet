using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using LiftNet.Utility.Utils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auths.Commands
{
    public class LogoutHandler : IRequestHandler<LogoutCommand, LiftNetRes>
    {
        private readonly IAuthRepo _authRepo;
        private readonly ILiftLogger<LogoutHandler> _logger;

        public LogoutHandler(IAuthRepo authRepo, ILiftLogger<LogoutHandler> logger)
        {
            _authRepo = authRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            _logger.Info($"attempt to logout user, {ContextUtil.Username}");
            await _authRepo.LogOutAsync();
            return LiftNetRes.SuccessResponse(message: "User logged out successfully.");
        }
    }
}

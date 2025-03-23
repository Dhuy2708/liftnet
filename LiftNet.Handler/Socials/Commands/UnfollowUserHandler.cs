using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Commands.Requets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Commands
{
    public class UnfollowUserHandler : IRequestHandler<UnfollowUserCommand, LiftNetRes>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILiftLogger<UnfollowUserHandler> _logger;

        public UnfollowUserHandler(IUnitOfWork uow, ILiftLogger<UnfollowUserHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            _logger.Info($"begin to unfollow user, userId: {request.TargetId}");

            var queryable = _uow.SocialConnectionRepo.GetQueryable();

            var socialConnection = queryable.FirstOrDefault(x => x.UserId == request.UserId && x.TargetId == request.TargetId);
            if (socialConnection == null)
            {
                _logger.Error($"dont follow this user");
                return LiftNetRes.ErrorResponse("You dont follow this user before");
            }

            socialConnection.Status = (int)SocialConnectionStatus.Unfollowing;

            var result = await _uow.CommitAsync();
            if (result > 0)
            {
                _logger.Info($"unfollow user success, userId: {request.TargetId}");
                return LiftNetRes.SuccessResponse("Unfollow user success");
            }
            _logger.Error($"unfollow user success, userId: {request.TargetId}");
            return LiftNetRes.ErrorResponse("Unfollow user failed");
        }
    }
}

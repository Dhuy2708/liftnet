using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Commands.Requets;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Commands
{
    public class FollowUserHandler : IRequestHandler<FollowUserCommand, LiftNetRes>
    {
        private readonly ILiftLogger<FollowUserHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<User> _userManager;

        public FollowUserHandler(ILiftLogger<FollowUserHandler> logger,
                                 IUnitOfWork socialConnectionRepo, 
                                 UserManager<User> userManager)
        {
            _logger = logger;
            _uow = socialConnectionRepo;
            _userManager = userManager;
        }

        public async Task<LiftNetRes> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var targetId = request.TargetId;
            var roles = await _userManager.GetRolesAsync(new User() { Id = targetId });
            if (roles.IsNotNullOrEmpty() && roles[0].Eq(LiftNetRoles.Admin))
            {
                return LiftNetRes.ErrorResponse("You can't follow an admin.");
            }

            var isFollowUserExist = await _uow.UserRepo.GetQueryable()
                                                       .AnyAsync(x => x.Id == targetId && !x.IsDeleted);      
            if (!isFollowUserExist)
            {
                return LiftNetRes.ErrorResponse("User not found.");
            }
            _logger.Info($"Begin to follow user, target: {targetId}");
            var queryable = _uow.SocialConnectionRepo.GetQueryable();
            var connection = queryable.FirstOrDefault(x => x.UserId == userId && x.TargetId == targetId);
            if (connection != null)
            {
                connection.Status = (int)SocialConnectionStatus.Following;
                await _uow.SocialConnectionRepo.Update(connection);
            }
            else
            {
                var newConnection = new SocialConnection()
                {
                    UserId = userId,
                    TargetId = targetId,
                    Status = (int)SocialConnectionStatus.Following,
                };
                await _uow.SocialConnectionRepo.Create(newConnection);
            }

            var result = await _uow.CommitAsync();
            if (result > 0)
            {
                _logger.Info("follow success");
                return LiftNetRes.SuccessResponse("Followed successfully.");
            }
            return LiftNetRes.ErrorResponse("Failed to follow.");
        }
    }
} 

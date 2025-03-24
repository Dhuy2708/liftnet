using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Queries
{
    public class ProfileHandler : IRequestHandler<UserProfileQuery, LiftNetRes<ProfileView>>
    {
        private readonly ILiftLogger<ProfileHandler> _logger;
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _uow;

        public ProfileHandler(ILiftLogger<ProfileHandler> logger, IUnitOfWork uow, IRoleService roleService)
        {
            _logger = logger;
            _uow = uow;
            _roleService = roleService;
        }

        public async Task<LiftNetRes<ProfileView>> Handle(UserProfileQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("get user profile");
            var profile = await _uow.UserRepo.GetQueryable().FirstOrDefaultAsync(x => x.Id == request.ProfileId);

            if (profile == null)
            {
                return LiftNetRes<ProfileView>.ErrorResponse("Profile not found.");
            }

            var socialQueryable = _uow.SocialConnectionRepo.GetQueryable();
            var followingCount = await socialQueryable.CountAsync(x => x.UserId == request.ProfileId && x.Status == (int)SocialConnectionStatus.Following);
            var followerCount = await socialQueryable.CountAsync(x => x.TargetId == request.ProfileId && x.Status == (int)SocialConnectionStatus.Following);
            var role = await _roleService.GetRoleByUserId(request.ProfileId);

            bool isFollowing = false;
            if (request.UserId.IsNotNullOrEmpty())
            {
                isFollowing = await socialQueryable.AnyAsync(x => x.UserId == request.UserId && 
                                                                  x.TargetId == request.ProfileId && 
                                                                  x.Status == (int)SocialConnectionStatus.Following);
            }

            var result = new ProfileView()
            {
                Id = profile.Id,
                IsSelf = request.IsSelf,
                UserName = profile.UserName!,
                Email = profile.Email!,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                IsFollowing = isFollowing,
                Role = role,
                Following = followingCount,
                Follower = followerCount,
                Avatar = profile.Avatar
            };

            return LiftNetRes<ProfileView>.SuccessResponse(result);
        }
    }
}

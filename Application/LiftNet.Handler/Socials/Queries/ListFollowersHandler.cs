using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Queries.Requests;
using LiftNet.Utility.Mappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Queries
{
    public class ListFollowersHandler : IRequestHandler<ListFollowersQuery, PaginatedLiftNetRes<UserOverview>>
    {
        private readonly ILiftLogger<ListFollowersHandler> _logger;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _uow;

        public ListFollowersHandler(ILiftLogger<ListFollowersHandler> logger, IUserService userService, IUnitOfWork uow)
        {
            _logger = logger;
            _userService = userService;
            _uow = uow;
        }

        public async Task<PaginatedLiftNetRes<UserOverview>> Handle(ListFollowersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cond = request.Conditions;
                var userId = cond.GetValue("userId");

                if (string.IsNullOrEmpty(userId))
                {
                    return PaginatedLiftNetRes<UserOverview>.ErrorResponse("userId is null or empty");
                }

                var queryable = _uow.SocialConnectionRepo.GetQueryable()
                                        .Where(x => x.TargetId == userId && x.Status == (int)SocialConnectionStatus.Following)
                                        .Select(x => x.UserId);

                var followerIds = await queryable.ToListAsync(cancellationToken);
                if (followerIds == null || !followerIds.Any())
                {
                    return PaginatedLiftNetRes<UserOverview>.SuccessResponse([], 0);
                }

                var userIdRoleDict = await _userService.GetUserIdRoleDict(followerIds);

                var followers = await _uow.UserRepo.GetQueryable()
                                                   .Include(x => x.UserRoles)
                                                   .Where(x => followerIds.Contains(x.Id))
                                                   .ToListAsync(cancellationToken);
                var result = followers.ToOverviews(userIdRoleDict);
                return PaginatedLiftNetRes<UserOverview>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing followers");
                return PaginatedLiftNetRes<UserOverview>.ErrorResponse("An error occurred while listing followers.");
            }
        }
    }
}

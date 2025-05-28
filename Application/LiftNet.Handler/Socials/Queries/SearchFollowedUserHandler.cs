using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using LiftNet.Utility.Utils;
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
    public class SearchFollowedUserHandler : IRequestHandler<SearchFollowedUserRequest, PaginatedLiftNetRes<UserOverview>>
    {
        private readonly ILiftLogger<SearchFollowedUserHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly RoleManager<Role> _roleManager;

        public SearchFollowedUserHandler(ILiftLogger<SearchFollowedUserHandler> logger, IUnitOfWork uow, RoleManager<Role> roleManager)
        {
            _logger = logger;
            _uow = uow;
            _roleManager = roleManager;
        }

        public async Task<PaginatedLiftNetRes<UserOverview>> Handle(SearchFollowedUserRequest request, CancellationToken cancellationToken)
        {
            _logger.Info("begin to search followed user");
            var userId = request.UserId;
            var cond = request.Conditions;

            var roles = _roleManager.Roles.ToList();

            var queryable = _uow.UserRepo.GetQueryable();
            queryable = queryable.Include(x => x.UserRoles);

            var notAdminRoles = roles.Where(x => !x.Name.Eq(LiftNetRoleEnum.Admin.ToString())).Select(x => x.Id).ToList();
            queryable = queryable.Where(x => notAdminRoles.Contains(x.UserRoles.First().RoleId));

            // followed filter
            var followedUserIds = await _uow.SocialConnectionRepo.GetQueryable()
                                           .Where(x => x.UserId == userId && x.Status == (int)SocialConnectionStatus.Following)
                                           .Select(x => x.TargetId)
                                           .ToListAsync();
            var bothFollowedUserIds = await _uow.SocialConnectionRepo.GetQueryable()
                                           .Where(x => x.TargetId == userId &&
                                                       followedUserIds.Contains(x.UserId) &&
                                                       x.Status == (int)SocialConnectionStatus.Following)
                                           .Select(x => x.UserId)
                                           .ToListAsync();

            queryable = queryable.Where(x => bothFollowedUserIds.Contains(x.Id));

            // search text
            var searchTxt = cond.Search;
            if (searchTxt.IsNotNullOrEmpty())
            {
                queryable = queryable.Where(x => x.UserName!.Contains(searchTxt!) ||
                                                 x.Email!.Contains(searchTxt!) ||
                                                 x.FirstName.Contains(searchTxt!) ||
                                                 x.LastName.Contains(searchTxt!));
            }

            // role filter
            var role = cond.FindCondition("role")?.Values.FirstOrDefault();
            if (role.IsNotNullOrEmpty() && Int32.TryParse(role, out var roleInt))
            {
                var roleStr = ((LiftNetRoleEnum)roleInt).ToString();
                var roleId = _roleManager.FindByNameAsync(roleStr).Result;
                queryable = queryable.Where(x => x.UserRoles.Any(r => r.RoleId == roleId!.Id));
            }


            
            queryable = queryable.OrderBy(x => x.UserName);
            queryable = queryable.BuildPaginated(cond);

            var users = await queryable.ToListAsync();
            var roleDict = roles.ToDictionary(x => x.Id, x => x.Name);
            Dictionary<string, LiftNetRoleEnum> roleEnumDict = new();

            foreach (var user in users)
            {
                if (user.UserRoles.FirstOrDefault() is { } userRole && roleDict.TryGetValue(userRole.RoleId, out var roleEnum))
                {
                    roleEnumDict[user.Id] = RoleUtil.GetRole(roleEnum!);
                }
            }
            var userOverviews = users.Select(x => x.ToOverview(roleEnumDict)).ToList();
            return PaginatedLiftNetRes<UserOverview>.SuccessResponse(userOverviews);
        }
    }
}

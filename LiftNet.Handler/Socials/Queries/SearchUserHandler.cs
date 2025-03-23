using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Searches.Queries.Requests;
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

namespace LiftNet.Handler.Searches.Queries
{
    public class SearchUserHandler : IRequestHandler<SearchUserQuery, PaginatedLiftNetRes<UserOverview>>
    {
        private readonly ILiftLogger<SearchUserHandler> _logger;
        private readonly IUserRepo _userRepo;
        private readonly RoleManager<Role> _roleManager;

        public SearchUserHandler(ILiftLogger<SearchUserHandler> logger, IUserRepo userRepo, RoleManager<Role> roleManager)
        {
            _logger = logger;
            _userRepo = userRepo;
            _roleManager = roleManager;
        }

        public async Task<PaginatedLiftNetRes<UserOverview>> Handle(SearchUserQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("begin to search user");
            var userId = request.UserId;
            var cond = request.Conditions;

            var queryable = _userRepo.GetQueryable();

            queryable = queryable.Include(x => x.UserRoles);

            // search text
            var searchTxt = cond.FindCondition("search")?.Values.FirstOrDefault();
            if (searchTxt.IsNotNullOrEmpty())
            {
                queryable = queryable.Where(x => x.UserName!.Contains(searchTxt!) || 
                                                 x.Email!.Contains(searchTxt!) ||
                                                 x.FirstName.Contains(searchTxt!) ||
                                                 x.LastName.Contains(searchTxt!));
            }

            // role filter
            var role = cond.FindCondition("role")?.Values.FirstOrDefault();
            if (role.IsNullOrEmpty())
            {
                return PaginatedLiftNetRes<UserOverview>.ErrorResponse("Role filter required");
            }
            if (Int32.TryParse(role, out var roleInt))
            {
                if (roleInt != (int)LiftNetRoleEnum.Seeker && roleInt != (int)LiftNetRoleEnum.Coach)
                {
                    return PaginatedLiftNetRes<UserOverview>.ErrorResponse("Invalid role");
                }
                var roleStr = ((LiftNetRoleEnum)roleInt).ToString();
                var roleId = _roleManager.FindByNameAsync(roleStr).Result;
                queryable = queryable.Where(x => x.UserRoles.Any(r => r.RoleId == roleId!.Id));
            }

            queryable = queryable.OrderBy(x => x.UserName);
            queryable = queryable.BuildPaginated(cond);


            var users = await queryable.ToListAsync();

            var roles = _roleManager.Roles.ToList();
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

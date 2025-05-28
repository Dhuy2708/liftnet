using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Response;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class UserController : LiftNetControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly RoleManager<Role> _roleManager;

        public UserController(IMediator mediator, IServiceProvider serviceProvider, IUnitOfWork uow, RoleManager<Role> roleManager) : base(mediator, serviceProvider)
        {
            _uow = uow;
            _roleManager = roleManager;
        }

        [HttpPost("roleMapping")]
        [ProducesResponseType(typeof(LiftNetRes<Dictionary<string, LiftNetRoleEnum>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRoleMapping(List<string> userIds)
        {
            var userRoleIds = await _uow.UserRepo.GetQueryable()
                                                 .Where(x => userIds.Contains(x.Id))
                                                                    .Select(x => new { userId = x.Id, roleId = x.UserRoles.FirstOrDefault()!.RoleId})
                                                                    .ToListAsync();

            var roleDict = (await _roleManager.Roles.ToListAsync()).ToDictionary(k => k.Id, v => v.Name);
            Dictionary<string, LiftNetRoleEnum> userRoleDict = userRoleIds.ToDictionary(k => k.userId, v => RoleUtil.GetRole(roleDict[v.roleId]!));
            if (userRoleDict.IsNotNullOrEmpty())
            {
                return Ok(LiftNetRes<Dictionary<string, LiftNetRoleEnum>>.SuccessResponse(userRoleDict));
            }
            return StatusCode(500, LiftNetRes<Dictionary<string, LiftNetRoleEnum>>.SuccessResponse(userRoleDict));
        }
    }
}

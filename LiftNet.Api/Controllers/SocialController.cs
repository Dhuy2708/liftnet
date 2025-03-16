using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.Searches.Queries.Requests;
using LiftNet.Handler.Socials.Commands.Requets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class SocialController : LiftNetControllerBase
    {
        public SocialController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("search")]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchUser(QueryCondition cond)
        {
            var req = new SearchUserQuery()
            {
                UserId = UserId,
                Conditions = cond,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("follow")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> FollowUser(string targetId)
        {
            var request = new FollowUserCommand()
            {
                UserId = UserId,
                TargetId = targetId,
            };
            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

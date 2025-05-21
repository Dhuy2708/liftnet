using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Commands.Requets;
using LiftNet.Handler.Socials.Queries.Requests;
using LiftNet.Utility.Extensions;
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

        [HttpPost("search/followed")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchFollowUser(QueryCondition cond)
        {
            var req = new SearchFollowedUserRequest()
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
            if (UserId.Eq(targetId))
            {
                throw new BadRequestException(["userId and targetId is equal"], "You cant follow yourself");
            }

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

        [HttpPost("unfollow")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnfollowUser(string targetId)
        {
            var request = new UnfollowUserCommand()
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

        [HttpGet("profile/{id}")]
        [ProducesResponseType(typeof(LiftNetRes<ProfileView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Profile([FromRoute] string id)
        {
            
            var req = new UserProfileQuery()
            {
                IsSelf = UserId.Eq(id),
                UserId = UserId,
                ProfileId = id,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("followings")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListFollowings(QueryCondition condition)
        {
            var req = new ListFollowingQuery()
            {
                CallerId = UserId,
                Conditions = condition,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("followers")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListFollowers(QueryCondition condition)
        {
            var req = new ListFollowersQuery()
            {
                CallerId = UserId,
                Conditions = condition,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("search/prioritized")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchPrioritizedUser(QueryCondition cond)
        {
            var req = new SearchPrioritizedUserQuery()
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

        [HttpGet("suggestFriends")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> FriendSuggestion()
        {
            var req = new FriendSuggestionQuery()
            {
                UserId = UserId,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }   
    }
}

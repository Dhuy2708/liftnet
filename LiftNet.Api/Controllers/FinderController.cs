using LiftNet.Api.Requests.Matchings;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using LiftNet.Handler.Finders.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class FinderController : LiftNetControllerBase
    {
        public FinderController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("postFinder")]
        [Authorize(Policy = LiftNetPolicies.Seeker)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostFinderRequest([FromBody] PostFinderRequest request)
        {
            var command = new PostFinderCommand
            {
                UserId = UserId,
                LocationId = request.LocationId,
                HideAddress = request.HideAddress,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                StartPrice = request.StartPrice,
                EndPrice = request.EndPrice,
                RepeatType = request.RepeatType,
                Title = request.Title,
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("apply")]
        [Authorize(Policy = LiftNetPolicies.Coach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ApplyFinder(ApplyFinderRequest request)
        {
            var command = new ApplyFinderPostCommand
            {
                UserId = UserId,
                PostId = request.PostId,
                Message = request.Message
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("list")]
        [Authorize(Policy = LiftNetPolicies.Seeker)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<FinderPostView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListFinderPosts([FromBody] QueryCondition conditions)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }

            var request = new ListFinderPostsQuery
            {
                Conditions = conditions,
                UserId = UserId
            };

            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("explore")]
        [Authorize(Policy = LiftNetPolicies.Coach)]
        [ProducesResponseType(typeof(LiftNetRes<List<FinderPostView>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ExploreFinderPosts()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }

            var request = new ExploreFinderPostsQuery
            {
                UserId = UserId
            };

            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("applicants/{postId}")]
        [Authorize(Policy = LiftNetPolicies.Seeker)]
        [ProducesResponseType(typeof(LiftNetRes<FinderPostApplicantView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListApplicants(string postId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }

            var request = new ListFinderPostApplicantsQuery
            {
                UserId = UserId,
                PostId = postId
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

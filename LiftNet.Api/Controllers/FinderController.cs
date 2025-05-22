using LiftNet.Api.Requests.Matchings;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.Matchings.Commands.Requests;
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
        public async Task<IActionResult> ApplyFinder([FromBody] ApplyFinderRequest request)
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
    }
}

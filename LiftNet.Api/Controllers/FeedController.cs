using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using LiftNet.Domain.Response;
using LiftNet.Domain.Indexes;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Constants;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : LiftNetControllerBase
    {
        public FeedController(IMediator mediator, IServiceProvider serviceProvider) 
            : base(mediator, serviceProvider)
        {
        }

        [HttpPost]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<FeedIndexData>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostFeed([FromBody] PostFeedRequest req)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var command = new PostFeedCommand
            {
                UserId = UserId,
                Content = req.Content,
                Medias = req.Medias
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<FeedIndexData>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateFeed([FromRoute] string id, [FromBody] UpdateFeedRequest req)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var command = new UpdateFeedCommand
            {
                Id = id,
                UserId = UserId,
                Content = req.Content,
                Medias = req.Medias
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteFeed([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var command = new DeleteFeedCommand
            {
                Id = id,
                UserId = UserId
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }
    }
} 
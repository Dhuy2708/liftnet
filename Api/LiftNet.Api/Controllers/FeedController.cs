using LiftNet.Api.Requests.Feeds;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums.Feed;
using LiftNet.Contract.Views.Feeds;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Handler.Feeds.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class FeedController : LiftNetControllerBase
    {
        public FeedController(IMediator mediator, IServiceProvider serviceProvider)
            : base(mediator, serviceProvider)
        {
        }

        [HttpPost("post")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<FeedIndexData>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostFeed([FromForm] PostFeedRequest req)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            if (req.MediaFiles == null && string.IsNullOrEmpty(req.Content))
            {
                return BadRequest(LiftNetRes.ErrorResponse("Either content or media files must be provided"));
            }

            var command = new PostFeedCommand
            {
                UserId = UserId,
                Content = req.Content,
                MediaFiles = req.MediaFiles ?? new List<IFormFile>()
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpPut("update/{id}")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<FeedIndexData>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateFeed([FromRoute] string id, [FromForm] UpdateFeedRequest req)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            if (req.MediaFiles != null && req.MediaFiles.Any(f => !f.ContentType.StartsWith("image/")))
            {
                return BadRequest(LiftNetRes.ErrorResponse("Only image files are allowed"));
            }

            var command = new UpdateFeedCommand
            {
                Id = id,
                UserId = UserId,
                Content = req.Content,
                MediaFiles = req.MediaFiles
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpDelete("delete/{id}")]
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

        [HttpPost("react")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReactFeed([FromBody] ReactFeedRequest req)
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            if (req.Type != ReactType.Like && req.Type != ReactType.UnLike)
            {
                return BadRequest(LiftNetRes.ErrorResponse("Invalid reaction type."));
            }

            var command = new ReactFeedCommand
            {
                FeedId = req.FeedId,
                FeedOwnerId = req.FeedOwnerId,
                UserId = UserId,
                Type = req.Type
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpPost("list/{userId}")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<List<FeedViewModel>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListFeeds([FromRoute] string userId, [FromBody] QueryCondition queryCondition)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(LiftNetRes.ErrorResponse("User ID is required"));

            var command = new ListFeedInProfileQuery
            {
                UserId = UserId,
                ProfileId = userId,
                QueryCondition = queryCondition
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpGet("list")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<FeedViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListRecommendedFeeds()
        {
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized();

            var command = new RecommendFeedsQuery
            {
                UserId = UserId
            };

            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }

        [HttpPost("comment")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CommentFeed(CommentFeedRequest req)
        {
            var command = new CommentFeedCommand
            {
                CallerId = UserId,
                Comment = req.Comment,
                FeedId = req.FeedId,
                ParentId = req.ParentId
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }

        [HttpGet("comments")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<CommentView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListComments([FromQuery] string feedId, [FromQuery] string? parentId = null)
        {
            if (string.IsNullOrEmpty(feedId))
                return BadRequest(LiftNetRes.ErrorResponse("Feed ID is required"));
            var command = new ListCommentsQuery
            {
                FeedId = feedId,
                ParentId = parentId
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
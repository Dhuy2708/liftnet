﻿using LiftNet.Api.Requests.Finders;
using LiftNet.Api.Requests.Matchings;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums.Finder;
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
            var command = new CreateFinderPostCommand
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
                IsAnonymous = request.IsAnonymous
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

        [HttpPost("cancel")]
        [Authorize(Policy = LiftNetPolicies.Coach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ApplyFinder(CancelFinderRequest request)
        {
            var command = new CancelFinderPostCommand
            {
                UserId = UserId,
                PostId = request.PostId,
                Reason = request.Reason
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
        [ProducesResponseType(typeof(LiftNetRes<ExploreFinderPostView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ExploreFinderPosts([FromQuery] float maxDistance)
        {
            if (maxDistance <= 0)
            {
                return BadRequest(LiftNetRes.ErrorResponse("invalid distance"));
            }
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }

            var request = new ExploreFinderPostsQuery
            {
                UserId = UserId,
                MaxDistance = maxDistance
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

        [HttpPost("applicant/response")]
        [Authorize(Policy = LiftNetPolicies.Seeker)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ResponseApplicant([FromBody] ResponseApplicantReq req)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            if (req.Status is FinderPostResponseType.None)
            {
                return BadRequest();
            }
            var request = new ResponseApplicantCommand
            {
                UserId = UserId,
                ApplicantId = req.ApplicantId,
                Status = req.Status
            };

            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("applieds")]
        [Authorize(Policy = LiftNetPolicies.Coach)]
        [ProducesResponseType(typeof(LiftNetRes<ExploreFinderPostView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListAppliedFinderPosts()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            var request = new ListAppliedFinderPostsQuery
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

        [HttpGet("applyMessage")]
        [Authorize(Policy = LiftNetPolicies.Coach)]
        [ProducesResponseType(typeof(LiftNetRes<AppliedFinderPostMessage>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AppliedFinderPostDetail([FromQuery] string postId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest("Post ID is required");
            }
            var request = new GetAppliedMessageQuery
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

        [HttpPost("recommendSeekerToPt")]
        public async Task<IActionResult> RecommendSeekerToPt(RecommendSeekerToPtReq postId)
        {
            var request = new RecommendSeekerToPtCommand
            {
                SeekerId = postId.SeekerId,
                PTIds = postId.PTIds,
                Description = postId.Description
            };
            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("seekerRecommendations")]
        [Authorize(Policy = LiftNetPolicies.Coach)]
        [ProducesResponseType(typeof(LiftNetRes<List<SeekerRecommendationView>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSeekerRecommendations()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }

            var request = new GetSeekerRecommendationsQuery
            {
                CoachId = UserId
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

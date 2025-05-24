using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Response;
using LiftNet.Handler.Conversations.Commands.Requests;
using LiftNet.Handler.Conversations.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class ConversationController : LiftNetControllerBase
    {
        public ConversationController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("list")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<ConversationOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListConversations([FromQuery] int pageNumber)
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            var request = new ListConversationsQuery
            {
                UserId = UserId,
                PageNumber = pageNumber
            };

            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateConversation([FromBody] string targetId)
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            var command = new CreateConversationCommand
            {
                UserId = UserId,
                TargetId = targetId
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("id")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetConversationId([FromQuery] string targetId)
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            var command = new CreateConversationCommand
            {
                UserId = UserId,
                TargetId = targetId
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("message/list")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<MessageView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListMessages([FromBody] QueryCondition conditions)
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            if (conditions == null)
            {
                return BadRequest();
            }

            var request = new ListMessagesQuery
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

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<ConversationInfo>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetConversationInfo([FromQuery] string conversationId)
        {
            var request = new GetConversationInfoQuery
            {
                ConversationId = conversationId,
                UserId = UserId
            };

            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("byUserId")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<ConversationInfo>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetConversationInfoByUserId([FromQuery] string targetId)
        {
            var request = new GetConversationInfoByUserIdQuery
            {
                UserId = UserId,
                TargetId = targetId
            };

            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("checkIfExist")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<bool>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckIfExist([FromQuery] string targetId)
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }
            var request = new CheckIfConversationExistQuery
            {
                UserId = UserId,
                TargetId = targetId
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

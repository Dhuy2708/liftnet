using LiftNet.Api.Requests.ChatBots;
using LiftNet.Contract.Views.Chatbots;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Commands.Requests;
using LiftNet.Handler.ChatBots.Queries.Requests;
using LiftNet.Redis.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class ChatBotController : LiftNetControllerBase
    {
        private IRedisSubService _subService => _serviceProvider.GetRequiredService<IRedisSubService>();
        public ChatBotController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("conversation/create")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateConversation([FromBody] CreateChatBotConversationReq req)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            var command = new CreateChatBotConversationCommand
            {
                UserId = UserId,
                Title = req.Title
            };
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("messages/{conversationId}")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<ChatbotMessageView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMessages(string conversationId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            var query = new ListChatbotMessagesQuery
            {
                UserId = UserId,
                ConversationId = conversationId
            };
            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

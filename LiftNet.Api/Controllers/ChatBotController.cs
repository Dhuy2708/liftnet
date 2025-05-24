using LiftNet.Api.Requests.ChatBots;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LiftNet.Api.Controllers
{
    public class ChatBotController : LiftNetControllerBase
    {
        public ChatBotController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("conversation/create")]
        [ProducesResponseType(typeof(LiftNetRes<string>), 200)]
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
    }
}

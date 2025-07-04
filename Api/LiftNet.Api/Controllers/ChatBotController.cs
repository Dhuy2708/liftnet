﻿using LiftNet.Api.Requests.ChatBots;
using LiftNet.Api.Responses;
using LiftNet.Contract.Dtos.Chatbot;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Chatbots;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Commands.Requests;
using LiftNet.Handler.ChatBots.Queries.Requests;
using LiftNet.Redis.Interface;
using LiftNet.Redis.Service;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class ChatBotController : LiftNetControllerBase
    {
        private IRedisSubService _subService => _serviceProvider.GetRequiredService<IRedisSubService>();
        private IChatbotService _chatbotService => _serviceProvider.GetRequiredService<IChatbotService>();

        public ChatBotController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("conversation/create")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<CreateChatbotConversationResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateConversation([FromBody] CreateChatBotConversationReq req)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            var command = new CreateChatBotConversationCommand
            {
                UserId = UserId,
                FirstPrompt = req.FirstPrompt
            };
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("conversations")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<ChatbotConversationView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListConversations()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return Unauthorized();
            }
            var command = new ListChatbotConversationsQuery
            {
                UserId = UserId,
            };
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("chat")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(ChatBotStateResponse), (int)HttpStatusCode.OK)]
        public async Task SendMessage([FromBody] SendChatBotMessageReq req)
        {
            var conversationId = req.ConversationId;
            var exist = await _chatbotService.CheckConversationAsync(UserId, conversationId);
            if (!exist)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("Conversation does not exist.");
            }
            UserIntent intent = UserIntent.None;

            var sessionId = conversationId;
            var channel = $"chatbot.session:{sessionId}";

            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";

            var cancellationToken = HttpContext.RequestAborted;

            async Task SendSseMessage(string message)
            {
                if (message.Eq("\""))
                {
                    return;
                }
                if ((message.StartsWith("'") && message.EndsWith("'")) || (message.StartsWith("\"") && message.EndsWith("\"")))
                {
                    message = message.Substring(1, message.Length - 2);
                }
                if (message == "DONE")
                {
                    return;
                }
                await Response.WriteAsync($"data: {message}");
                await Response.Body.FlushAsync();
            }

            await _subService.SubscribeAsync(channel, async (redisChannel, redisValue) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    await _subService.UnsubscribeAsync(channel);
                    return;
                }

                var str = redisValue.ToString();
                if (str.StartsWith("[INTENT]"))
                {
                    var intentStr = str.Substring("[INTENT]".Length).Trim();
                    intent = intentStr switch
                    {
                        "general_knowledge" => UserIntent.GeneralKnowledge,
                        "personal_advice" => UserIntent.PersonalAdvice,
                        "training_plan_update" => UserIntent.UpdatePlan,
                        _ => UserIntent.None,
                    };
                }
                else
                {
                    await SendSseMessage(str);
                }
            });


            try
            {
                var chatTask = _chatbotService.ChatAsync(UserId, conversationId, req.Message);
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (chatTask.IsCompleted)
                        break;

                    await Task.Delay(100, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                await Response.WriteAsync($"data: [ERROR]");
            }
            finally
            {
                await _subService.UnsubscribeAsync(channel);
            }
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

using LiftNet.Contract.Enums.Conversation;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Hub.Constant;
using LiftNet.Hub.Contract;
using LiftNet.Hub.Provider;
using LiftNet.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Core
{
    [Authorize]
    public class ChatHub : BaseHub<ChatMessage>
    {
        private readonly IChatIndexService _chatService;
        private readonly ILiftLogger<ChatHub> _logger;
        private readonly IConversationRepo _conversationRepo;

        public ChatHub(ConnectionPool connPool,
                       IChatIndexService chatService,
                       IConversationRepo conversationRepo,
                       ILiftLogger<ChatHub> logger) 
            : base(connPool, HubNames.chat)
        {
            _chatService = chatService;
            _conversationRepo = conversationRepo;
            _logger = logger;
        }

        public async Task SendMessage(List<string> recieverIds, ChatMessage message)
        {
            try
            {
                string? msgId = null;
                try
                {
                    _logger.Info($"sending message to userid : {recieverIds}");
                    if (message.ConversationId.IsNullOrEmpty() || message.Body.IsNullOrEmpty())
                    {
                        return;
                    }
                    msgId = await _chatService.SaveMessages(message.ConversationId, CallerId, message.Body, message.Type, message.Time);
                    if (msgId.IsNotNullOrEmpty())
                    {
                        message.MessageId = msgId;
                    }
                    var conversation = await _conversationRepo.GetById(message.ConversationId);
                    if (conversation != null)
                    {
                        conversation.LastUpdate = DateTime.UtcNow;
                        await _conversationRepo.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "error while saving message");
                    await PingMessageStatus(message.TrackId, ChatMessageStatus.Failed);
                    return;
                }

                var pingTask = PingMessageStatus(message.TrackId, ChatMessageStatus.Sent, msgId);
                var sendTask = SendToUsers(recieverIds, message);
                await Task.WhenAll(pingTask, sendTask);
            }
            catch (Exception e)
            {
                _logger.Error(e, "error while sending message");
                return;
            }
        }

        private async Task PingMessageStatus(string trackId, ChatMessageStatus status, string? messageId = null)
        {
            await SendToCaller(new
            {
                TrackId = trackId,
                MessageId = messageId,
                Status = (int)status
            }, "MessageSent");
        }
    }
}

using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Chatbots;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.ChatBots.Queries
{
    public class ListChatbotMessageHandler : IRequestHandler<ListChatbotMessagesQuery, LiftNetRes<ChatbotMessageView>>
    {
        private readonly ILiftLogger<ListChatbotMessageHandler> _logger;
        private readonly IChatBotMessageRepo _messageRepo;

        public ListChatbotMessageHandler(ILiftLogger<ListChatbotMessageHandler> logger, IChatBotMessageRepo messageRepo)
        {
            _logger = logger;
            _messageRepo = messageRepo;
        }

        public async Task<LiftNetRes<ChatbotMessageView>> Handle(ListChatbotMessagesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;
                var conversationId = request.ConversationId;
                var queryable = _messageRepo.GetQueryable();

                var messages = await queryable.Where(x => x.ChatBotConversation.UserId == userId &&
                                                    x.ConversationId == conversationId)
                                            .OrderBy(x => x.Time)
                                            .Select(x => new ChatbotMessageView
                                            {
                                                Id = x.Id,
                                                ConversationId = x.ConversationId,
                                                IsHuman = x.IsHuman,
                                                Message = x.Message,
                                                Time = new DateTimeOffset(x.Time, TimeSpan.Zero)
                                            })
                                            .ToListAsync(cancellationToken);

                if (messages == null || !messages.Any())
                {
                    return LiftNetRes<ChatbotMessageView>.SuccessResponse([]);
                }
                return LiftNetRes<ChatbotMessageView>.SuccessResponse(messages);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"error while listing chatbot messages");
                return LiftNetRes<ChatbotMessageView>.ErrorResponse("An error occurred while retrieving messages.");
            }
        }
    }
}

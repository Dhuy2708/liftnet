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
    public class ListChatbotConversationHandler : IRequestHandler<ListChatbotConversationsQuery, LiftNetRes<ChatbotConversationView>>
    {
        private readonly ILiftLogger<ListChatbotConversationHandler> _logger;
        private readonly IChatBotConversationRepo _conversationRepo;

        public ListChatbotConversationHandler(ILiftLogger<ListChatbotConversationHandler> logger, IChatBotConversationRepo conversationRepo)
        {
            _logger = logger;
            _conversationRepo = conversationRepo;
        }

        public async Task<LiftNetRes<ChatbotConversationView>> Handle(ListChatbotConversationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var conversations = await _conversationRepo.GetQueryable()
                                                .Where(x => x.UserId == request.UserId)
                                                .OrderByDescending(x => x.LastUpdate)
                                                .ToListAsync();
                var result = conversations.Select(x => new ChatbotConversationView
                {
                    Id = x.Id,
                    Title = x.Title,
                }).ToList();
                return LiftNetRes<ChatbotConversationView>.SuccessResponse(result); 
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing chatbot conversations.");
                return LiftNetRes<ChatbotConversationView>.ErrorResponse("An error occurred while processing your request.");
            }
        }
    }
}

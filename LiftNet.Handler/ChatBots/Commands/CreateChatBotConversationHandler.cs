using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Commands.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.ChatBots.Commands
{
    public class CreateChatBotConversationHandler : IRequestHandler<CreateChatBotConversationCommand, LiftNetRes<string>>
    {
        private readonly ILiftLogger<CreateChatBotConversationHandler> _logger;
        private readonly IChatBotConversationRepo _conversationRepo;

        public CreateChatBotConversationHandler(ILiftLogger<CreateChatBotConversationHandler> logger, IChatBotConversationRepo conversationRepo)
        {
            _logger = logger;
            _conversationRepo = conversationRepo;
        }

        public async Task<LiftNetRes<string>> Handle(CreateChatBotConversationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = new ChatBotConversation()
                {
                    Id = Guid.NewGuid().ToString(),
                    LastUpdate = DateTime.UtcNow,
                    Title = request.Title,
                    UserId = request.UserId,
                };
                await _conversationRepo.Create(entity);
                return LiftNetRes<string>.SuccessResponse(entity.Id, message: "Conversation created successfully" );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating chat bot conversation");
                return LiftNetRes<string>.ErrorResponse("An error occurred while creating the conversation");
            }
        }
    }
}

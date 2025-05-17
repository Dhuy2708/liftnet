using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Conversations.Commands.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Conversations.Commands
{
    public class CreateConversationHandler : IRequestHandler<CreateConversationCommand, LiftNetRes<string>>
    {
        private readonly IConversationRepo _conversationRepo;
        private readonly ILiftLogger<CreateConversationHandler> _logger;

        public CreateConversationHandler(
            IConversationRepo conversationRepo,
            ILiftLogger<CreateConversationHandler> logger)
        {
            _conversationRepo = conversationRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes<string>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            _logger.Info("begin create conversation");
            if (request.UserId.Eq(request.TargetId))
            {
                return LiftNetRes<string>.ErrorResponse("cant create conversation with yourself");
            }
            // Check if conversation already exists
            var existingConversation = await _conversationRepo.GetQueryable()
                .Where(x => !x.IsGroup && 
                    ((x.UserId1 == request.UserId && x.UserId2 == request.TargetId) ||
                     (x.UserId1 == request.TargetId && x.UserId2 == request.UserId)))
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                return LiftNetRes<string>.ErrorResponse("conversation already exists");
            }

            var conversation = new Conversation
            {
                UserId1 = request.UserId,
                UserId2 = request.TargetId,
                IsGroup = false,
                CreatedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow
            };

            await _conversationRepo.Create(conversation);
            _logger.Info("create conversation successfully");
            return LiftNetRes<string>.SuccessResponse(conversation.Id);
        }
    }
} 
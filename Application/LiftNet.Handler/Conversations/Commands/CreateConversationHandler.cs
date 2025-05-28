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
        private readonly IUserRepo _userRepo;
        private readonly ILiftLogger<CreateConversationHandler> _logger;

        public CreateConversationHandler(
            IConversationRepo conversationRepo,
            IUserRepo userRepo,
            ILiftLogger<CreateConversationHandler> logger)
        {
            _conversationRepo = conversationRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes<string>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
        {
            _logger.Info("begin create conversation");
            if (request.UserId.Eq(request.TargetId))
            {
                return LiftNetRes<string>.ErrorResponse("cant create conversation with yourself");
            }

            var isUserExist = await _userRepo.GetQueryable()
                                       .AnyAsync(x => x.Id == request.TargetId, cancellationToken);

            if (!isUserExist)
            {
                return LiftNetRes<string>.ErrorResponse("target user not found");
            }

            // Check if conversation already exists
            var existingConversation = await _conversationRepo.GetQueryable()
                .Where(x => !x.IsGroup && 
                    ((x.UserId1 == request.UserId && x.UserId2 == request.TargetId) ||
                     (x.UserId1 == request.TargetId && x.UserId2 == request.UserId)))
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                return LiftNetRes<string>.SuccessResponse(existingConversation.Id, "conversation already exists");
            }

            var timeCreated = DateTime.UtcNow;
            var conversation = new Conversation
            {
                UserId1 = request.UserId,
                UserId2 = request.TargetId,
                IsGroup = false,
                CreatedAt = timeCreated,
                LastUpdate = timeCreated
            };

            await _conversationRepo.Create(conversation);
            _logger.Info("create conversation successfully");
            return LiftNetRes<string>.SuccessResponse(conversation.Id);
        }
    }
} 
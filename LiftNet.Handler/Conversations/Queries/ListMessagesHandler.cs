using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Conversations.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Conversations.Queries
{
    public class ListMessagesHandler : IRequestHandler<ListMessagesQuery, PaginatedLiftNetRes<MessageView>>
    {
        private readonly IChatIndexService _chatService;
        private readonly IConversationRepo _conversationRepo;
        private readonly ILiftLogger<ListMessagesHandler> _logger;

        public ListMessagesHandler(
            IChatIndexService chatService,
            IConversationRepo conversationRepo,
            ILiftLogger<ListMessagesHandler> logger)
        {
            _chatService = chatService;
            _conversationRepo = conversationRepo;
            _logger = logger;
        }

        public async Task<PaginatedLiftNetRes<MessageView>> Handle(ListMessagesQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("begin list messages");

            if (request.Conditions == null)
            {
                request.Conditions = new QueryCondition();
            }
            var conversationId = request.Conditions.FindCondition("conversationid")?.Values.FirstOrDefault();

            if (string.IsNullOrEmpty(conversationId))
            {
                return PaginatedLiftNetRes<MessageView>.ErrorResponse("conversationId is required");
            }

            if (! await _conversationRepo.IsConversationExist(conversationId, request.UserId))
            {
                return PaginatedLiftNetRes<MessageView>.ErrorResponse("conversation is not exist");
            }

            var (messages, nextPageToken) = await _chatService.GetMessages(
                conversationId,
                request.Conditions.PageSize,
                request.Conditions.NextPageToken);

            var messageViews = messages.Select(x => new MessageView
            {
                Id = x.Id,
                SenderId = x.UserId,
                Type = x.Type,
                Body = x.Message,
                Time = new DateTimeOffset(x.CreatedAt, TimeSpan.Zero)
            }).ToList();

            _logger.Info("list messages successfully");
            return PaginatedLiftNetRes<MessageView>.SuccessResponse(messageViews, nextPageToken: nextPageToken);
        }
    }
} 
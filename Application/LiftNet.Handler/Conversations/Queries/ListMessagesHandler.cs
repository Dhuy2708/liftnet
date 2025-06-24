using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Entities;
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
        private readonly IChatSeenStatusRepo _seenRepo;
        private readonly ILiftLogger<ListMessagesHandler> _logger;

        public ListMessagesHandler(
            IChatIndexService chatService,
            IConversationRepo conversationRepo,
            IChatSeenStatusRepo seenRepo,
            ILiftLogger<ListMessagesHandler> logger)
        {
            _chatService = chatService;
            _conversationRepo = conversationRepo;
            _seenRepo = seenRepo;
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
            await UpdateNotiCount(request.UserId, conversationId);
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

        private async Task UpdateNotiCount(string userId, string conversationId)
        {
            var notiRecord = await _seenRepo.GetQueryable()
                                            .FirstOrDefaultAsync(x => x.UserId == userId &&
                                                                 x.ConversationId == conversationId);
            var now = DateTime.UtcNow;
            if (notiRecord == null)
            {
                var newRecord = new ChatSeenStatus
                {
                    UserId = userId,
                    ConversationId = conversationId,
                    NotiCount = 0,
                    LastSeen = now,
                    LastUpdate = now
                };
                await _seenRepo.Create(newRecord);
            }
            else
            {
                notiRecord.NotiCount = 0;
                notiRecord.LastSeen = now;
                notiRecord.LastUpdate = now;
                await _seenRepo.Update(notiRecord);
            }
        }
    }
} 
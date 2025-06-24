using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Conversations.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace LiftNet.Handler.Conversations.Queries
{
    public class ListConversationsHandler : IRequestHandler<ListConversationsQuery, PaginatedLiftNetRes<ConversationOverview>>
    {
        private readonly IConversationRepo _conversationRepo;
        private readonly IConversationUserRepo _conversationUserRepo;
        private readonly IRoleService _roleService;
        private readonly IChatIndexService _chatService;
        private readonly IChatSeenStatusRepo _seenRepo;
        private readonly ILiftLogger<ListConversationsHandler> _logger;

        public ListConversationsHandler(
                    IConversationRepo conversationRepo,
                    IConversationUserRepo conversationUserRepo,
                    IChatIndexService chatService,
                    IRoleService roleService,
                    IChatSeenStatusRepo seenRepo,
                    ILiftLogger<ListConversationsHandler> logger)
        {
            _conversationRepo = conversationRepo;
            _conversationUserRepo = conversationUserRepo;
            _chatService = chatService;
            _roleService = roleService;
            _seenRepo = seenRepo;
            _logger = logger;
        }

        public async Task<PaginatedLiftNetRes<ConversationOverview>> Handle(ListConversationsQuery request, CancellationToken cancellationToken)
        {
            var pageSize = 10;
            var userId = request.UserId;
            var conversations = await _conversationRepo.GetQueryable()
                                        .Include(x => x.User1)
                                        .ThenInclude(x => x.UserRoles)
                                        .Include(x => x.User2)
                                        .ThenInclude(x => x.UserRoles)
                                        .Where(x => (x.UserId1 == userId || x.UserId2 == userId) &&
                                                    x.CreatedAt != x.LastUpdate)
                                        .OrderByDescending(x => x.LastUpdate)
                                        .Skip((request.PageNumber - 1) * pageSize)
                                        .ToListAsync();

            var roleDict = await _roleService.GetAllRoleDictAsync();
            var conversationIds = conversations.Select(x => x.Id).ToList();

            var tasks = conversationIds
                            .Select(conId => _chatService.GetLastMessage(conId))
                            .ToList();

            var lastMessages = (await Task.WhenAll(tasks)).ToList();
            var lastMessagesDict = lastMessages.Where(x => x != null)
                                                .ToDictionary(x => x.ConversationId, x => x);

            List<ConversationOverview> result = [];
            var notiCountDict = await _seenRepo.GetQueryable()
                                         .Where(x => x.UserId == userId &&
                                                     conversationIds.Contains(x.ConversationId))
                                         .ToDictionaryAsync(k => k.ConversationId, v => v.NotiCount);
            foreach (var conversation in conversations)
            {
                var user1 = conversation.User1;
                var user2 = conversation.User2;
                var targetUser = conversation.UserId1 == userId ? user2 : user1;
                var lastMessage = lastMessagesDict.GetValueOrDefault(conversation.Id) ?? null;
                result.Add(new ConversationOverview
                {
                    Id = conversation.Id,
                    Img = conversation.UserId1 == userId ? conversation.User2!.Avatar : conversation.User1!.Avatar,
                    IsGroup = false,
                    LastMessage = lastMessage != null ? new MessageView
                    {
                        Body = lastMessage.Message,
                        Id = lastMessage.Id,
                        SenderId = lastMessage.UserId,
                        Type = lastMessage.Type,
                        Time = new DateTimeOffset(lastMessage.CreatedAt, TimeSpan.Zero),
                    } : null,
                    Name = conversation.UserId1 == userId
                                        ? user2.FirstName + user2.LastName
                                        : user1.FirstName + user1.LastName,
                    Role = roleDict.GetValueOrDefault(targetUser.UserRoles.First()!.RoleId),
                    NotiCount = notiCountDict.GetValueOrDefault(conversation.Id, 0),
                });
            }

            return PaginatedLiftNetRes<ConversationOverview>.SuccessResponse(result, pageNumber: request.PageNumber);
        }
    }
} 
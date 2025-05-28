using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Conversations;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Conversations.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Conversations.Queries
{
    public class GetConversationInfoByUserIdHandler : IRequestHandler<GetConversationInfoByUserIdQuery, LiftNetRes<ConversationInfo>>
    {
        private readonly IConversationRepo _conversationRepo;
        private readonly IUserService _userService;
        private readonly ILiftLogger<GetConversationInfoHandler> _logger;

        public GetConversationInfoByUserIdHandler(IConversationRepo conversationRepo,
                                          IUserService userService,
                                          ILiftLogger<GetConversationInfoHandler> logger)
        {
            _conversationRepo = conversationRepo;
            _userService = userService;
            _logger = logger;
        }

        public async Task<LiftNetRes<ConversationInfo>> Handle(GetConversationInfoByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var targetId = request.TargetId;

            if (targetId.IsNullOrEmpty() || userId.IsNullOrEmpty())
            {
                return LiftNetRes<ConversationInfo>.ErrorResponse("targetId is required");
            }

            var isExist = await _conversationRepo.IsConversationExistByUserId(userId, targetId);

            if (!isExist)
            {
                return LiftNetRes<ConversationInfo>.ErrorResponse("conversation not found");
            }
            var conversation = await _conversationRepo.GetQueryable()
                                .Include(x => x.User1)
                                .ThenInclude(x => x.UserRoles)
                                .Include(x => x.User2)
                                .ThenInclude(x => x.UserRoles)
                                .FirstAsync(x => (x.UserId1 == userId && x.UserId2 == targetId) ||
                                                 (x.UserId1 == targetId && x.UserId2 == userId));

            var user1 = conversation.User1;
            var user2 = conversation.User2;
            var targetUser = user1.Id == userId ? user2 : user1;
            var result = new ConversationInfo()
            {
                Id = conversation.Id,
                IsGroup = conversation.IsGroup,
                Name = targetUser.FirstName + " " + targetUser.LastName,
                OtherMembers = []
            };

            var userRoleDict = await _userService.GetUserIdRoleDict([user1.Id, user2.Id]);
            result.OtherMembers.Add(new UserOverview()
            {
                Id = targetUser.Id,
                Email = targetUser.Email!,
                Username = targetUser.UserName!,
                Avatar = targetUser.Avatar,
                FirstName = targetUser.FirstName,
                LastName = targetUser.LastName,
                Role = userRoleDict.GetValueOrDefault(user1.Id),
                IsDeleted = targetUser.IsDeleted,
                IsSuspended = targetUser.IsSuspended,
            });

            return LiftNetRes<ConversationInfo>.SuccessResponse(result);
        }

    }
}

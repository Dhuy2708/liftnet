using LiftNet.Contract.Interfaces.IRepos;
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
    public class CheckIfConversationExistHandler : IRequestHandler<CheckIfConversationExistQuery, LiftNetRes<bool>>
    {
        private readonly ILiftLogger<CheckIfConversationExistHandler> _logger;
        private readonly IConversationRepo _conversationRepo;

        public CheckIfConversationExistHandler(ILiftLogger<CheckIfConversationExistHandler> logger, IConversationRepo conversationRepo)
        {
            _logger = logger;
            _conversationRepo = conversationRepo;
        }

        public async Task<LiftNetRes<bool>> Handle(CheckIfConversationExistQuery request, CancellationToken cts)
        {
            _logger.Info("begin check if conversation exist");
            
            var isExist = await _conversationRepo.GetQueryable()
                .AnyAsync(x => (x.UserId1 == request.UserId && x.UserId2 == request.TargetId) ||
                     (x.UserId1 == request.TargetId && x.UserId2 == request.UserId), cts);

            return LiftNetRes<bool>.SuccessResponse(isExist);
        }
    }
}

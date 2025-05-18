using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Conversations.Queries.Requests
{
    public class GetConversationInfoQuery : IRequest<LiftNetRes<ConversationInfo>>
    {
        public string UserId
        {
            get; set;
        }
        public string ConversationId
        {
            get; set;
        }
    }

    public class GetConversationInfoByUserIdQuery : IRequest<LiftNetRes<ConversationInfo>>
    {
        public string UserId
        {
            get; set;
        }
        public string TargetId
        {
            get; set;
        }
    }
}

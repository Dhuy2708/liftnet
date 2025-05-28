using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Conversations.Queries.Requests
{
    public class ListConversationsQuery : IRequest<PaginatedLiftNetRes<ConversationOverview>>
    {
        public string UserId 
        {
            get; set;
        }

        public int PageNumber
        {
            get; set;
        }
    }
} 
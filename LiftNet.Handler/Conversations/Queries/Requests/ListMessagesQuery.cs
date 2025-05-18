using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Conversations;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Conversations.Queries.Requests
{
    public class ListMessagesQuery : IRequest<PaginatedLiftNetRes<MessageView>>
    {
        public QueryCondition Conditions 
        { 
            get; set; 
        }
        public string UserId
        {
            get; set;
        }
    }
} 
using MediatR;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Response;
using LiftNet.Contract.Dtos.Query;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class ListFeedCommand : IRequest<LiftNetRes<List<FeedIndexData>>>
    {
        public string UserId { get; set; }
        public QueryCondition QueryCondition { get; set; }
    }
} 
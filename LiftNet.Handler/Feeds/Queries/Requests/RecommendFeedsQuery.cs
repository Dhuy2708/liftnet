using LiftNet.Contract.Views.Feeds;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Feeds.Queries.Requests
{
    public class RecommendFeedsQuery : IRequest<LiftNetRes<FeedViewModel>>
    {
        public string UserId { get; set; }
    }
}
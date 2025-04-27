using MediatR;
using LiftNet.Domain.Response;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Domain.ViewModels;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class ListFeedCommand : IRequest<LiftNetRes<FeedViewModel>>
    {
        public string UserId { get; set; }
        public QueryCondition QueryCondition { get; set; }
    }
} 
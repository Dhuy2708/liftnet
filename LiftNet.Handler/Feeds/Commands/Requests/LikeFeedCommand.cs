using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class LikeFeedCommand : IRequest<LiftNetRes>
    {
        public string FeedId { get; set; }
        public string UserId { get; set; }
    }
}
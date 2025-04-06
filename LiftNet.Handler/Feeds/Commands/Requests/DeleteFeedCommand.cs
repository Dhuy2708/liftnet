using MediatR;
using LiftNet.Domain.Response;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class DeleteFeedCommand : IRequest<LiftNetRes>
    {
        public string Id { get; set; }
        public string UserId { get; set; }
    }
} 
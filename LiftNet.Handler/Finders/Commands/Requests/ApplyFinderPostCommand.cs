using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Finders.Commands.Requests
{
    public class ApplyFinderPostCommand : IRequest<LiftNetRes>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string Message { get; set; }
    }
} 
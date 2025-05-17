using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Conversations.Commands.Requests
{
    public class CreateConversationCommand : IRequest<LiftNetRes<string>>
    {
        public string UserId { get; set; }
        public string TargetId { get; set; }
    }
} 
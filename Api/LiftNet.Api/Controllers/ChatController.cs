using MediatR;

namespace LiftNet.Api.Controllers
{
    public class ChatController : LiftNetControllerBase
    {
        public ChatController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }
    }
}

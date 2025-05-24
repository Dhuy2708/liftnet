using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.ChatBots.Commands.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.ChatBots.Commands
{
    public class CreateChatBotConversationHandler : IRequestHandler<CreateChatBotConversationCommand, LiftNetRes>
    {
        private readonly ILiftLogger<CreateChatBotConversationHandler> _logger;

        public Task<LiftNetRes> Handle(CreateChatBotConversationCommand request, CancellationToken cancellationToken)
        {
            
        }
    }
}

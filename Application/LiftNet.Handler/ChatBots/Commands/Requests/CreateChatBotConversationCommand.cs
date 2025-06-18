using LiftNet.Contract.Dtos.Chatbot;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.ChatBots.Commands.Requests
{
    public class CreateChatBotConversationCommand : IRequest<LiftNetRes<CreateChatbotConversationResponse>>
    {
        public string UserId
        {
            get; set;
        }
        public string FirstPrompt
        {
            get; set;
        }
    }
}

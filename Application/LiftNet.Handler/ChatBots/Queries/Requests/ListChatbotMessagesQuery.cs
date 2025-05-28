using LiftNet.Contract.Views.Chatbots;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.ChatBots.Queries.Requests
{
    public class ListChatbotMessagesQuery : IRequest<LiftNetRes<ChatbotMessageView>>
    {
        public string UserId
        {
            get; set;
        }

        public string ConversationId
        {
            get; set;
        }
    }
}

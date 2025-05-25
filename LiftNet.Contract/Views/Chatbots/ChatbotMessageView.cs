using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Chatbots
{
    public class ChatbotMessageView
    {
        public string Id
        {
            get; set;
        }
        public string ConversationId
        {
            get; set;
        }
        public string Message
        {
            get; set;
        }
        public DateTimeOffset Time
        {
            get; set;
        }
        public bool IsHuman
        {
            get; set;
        }
    }
}

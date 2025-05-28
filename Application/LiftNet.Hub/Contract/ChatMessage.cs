using LiftNet.Domain.Enums.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Contract
{
    public class ChatMessage : HubMessage
    {
        public string TrackId
        {
            get; set;
        }
        public string? MessageId
        {
            get; set;
        }
        public string ConversationId
        {
            get; set;
        }
        public DateTime Time
        {
            get; set;
        }
        public ChatMessageType Type
        {
            get; set;
        }
    }
}

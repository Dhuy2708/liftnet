using LiftNet.Domain.Enums.Indexes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Indexes
{
    public class ChatMessageIndexData : IndexData
    {
        [JsonProperty(PropertyName = "conversationid")]
        public string ConversationId
        {
            get; set;
        }

        [JsonProperty(PropertyName = "message")]
        public string Message
        {
            get; set;
        }

        [JsonProperty(PropertyName = "type")]
        public ChatMessageType Type
        {
            get; set;
        }
    }
}

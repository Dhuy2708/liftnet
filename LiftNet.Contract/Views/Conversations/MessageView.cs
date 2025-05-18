using LiftNet.Domain.Enums.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Conversations
{
    public class MessageView
    {
        public string Id
        {
            get; set;
        }
        public string SenderId
        {
            get; set;
        }
        public ChatMessageType Type
        {
            get; set;
        }
        public string Body
        {
            get; set;
        }
        public DateTimeOffset Time
        {
            get; set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Contract
{
    public class ChatMessage : HubMessage
    {
        public DateTimeOffset Time
        {
            get; set;
        }
    }
}

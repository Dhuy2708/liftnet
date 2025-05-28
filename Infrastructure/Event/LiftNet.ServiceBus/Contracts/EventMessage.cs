using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Contracts
{
    public class EventMessage
    {
        public EventType Type
        {
            get; set;
        }
        public string Context
        {
            get; set;
        } = string.Empty;
    }
}

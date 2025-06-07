using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Contract
{
    public class NotiMessage : HubMessage
    {
        public string Title
        {
            get; set;
        } = string.Empty;

        public string SenderUsername
        {
            get; set;
        } = string.Empty;

        public string SenderName
        {
            get; set;
        } = string.Empty;
     
        public string SenderAvatar
        {
            get; set;
        } = string.Empty;

        public NotiTarget SenderType
        {
            get; set;
        }

        public string? RecieverId
        {
            get; set;
        }


        public NotiTarget RecieverType
        {
            get; set;
        }

        public List<string> ObjectNames
        {
            get; set;
        } = [];

        public NotiEventType EventType
        {
            get; set;
        }
    }
}

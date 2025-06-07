using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Notis
{
    public class NotificationView
    {
        public int Id
        {
            get; set;
        }

        public string UserId
        {
            get; set;
        }

        public string? SenderName
        {
            get; set;
        }

        public int SenderType
        {
            get; set;
        }

        public string SenderAvatar
        {
            get; set;
        } = string.Empty;

        public int RecieverType
        {
            get; set;
        }

        public string? Title
        {
            get; set;
        }

        public NotiEventType EventType
        {
            get; set;
        }

        public string? Body
        {
            get; set;
        } = string.Empty;

        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;

        public NotiRefernceLocationType Location
        {
            get; set;
        }
    }
}

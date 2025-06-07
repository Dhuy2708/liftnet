using LiftNet.Contract.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos
{
    public class NotiMessageDto
    {
        public string Title
        {
            get; set;
        } = string.Empty;

        public string SenderId
        {
            get; set;
        } = string.Empty;

        public string Body
        {
            get; set;
        } = string.Empty;

        public string? RecieverId
        {
            get; set;
        }

        public NotiTarget SenderType
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
        public NotiRefernceLocationType Location
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        } = DateTime.UtcNow;
    }
}

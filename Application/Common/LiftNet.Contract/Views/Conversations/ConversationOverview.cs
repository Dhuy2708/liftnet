using LiftNet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Conversations
{
    public class ConversationOverview
    {
        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Img
        {
            get; set;
        }

        public bool IsGroup
        {
            get; set;
        }

        public LiftNetRoleEnum? Role
        {
            get; set;
        }

        public MessageView? LastMessage
        {
            get; set;
        }
    }
}

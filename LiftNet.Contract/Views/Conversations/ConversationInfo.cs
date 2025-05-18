using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Conversations
{
    public class ConversationInfo
    {
        public string Id
        {
            get; set;
        }
        public List<UserOverview> Members
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public bool IsGroup
        {
            get; set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Contract
{
    public class HubMessage
    {
        public string SenderId
        {
            get; set;
        } = string.Empty;

        public string Body
        {
            get; set;
        } = string.Empty;

        public DateTime CreatedAt
        {
            get; set;
        }
    }
}

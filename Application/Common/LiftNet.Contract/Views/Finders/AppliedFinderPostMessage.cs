using LiftNet.Contract.Enums.Finder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Finders
{
    public class AppliedFinderPostMessage
    {
        public string PostId
        {
            get; set;
        }
        public string? ApplyMessage
        {
            get; set;
        }
        public string? CancelReason
        {
            get; set;
        }
        public DateTimeOffset ModifiedAt // use last modified
        {
            get; set;
        }
    }
}

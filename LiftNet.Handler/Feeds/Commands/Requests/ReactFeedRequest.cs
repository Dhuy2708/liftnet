using LiftNet.Contract.Enums.Feed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class ReactFeedRequest
    {
        public string FeedId { get; set; } = string.Empty;
        public ReactType Type { get; set; }
    }
} 
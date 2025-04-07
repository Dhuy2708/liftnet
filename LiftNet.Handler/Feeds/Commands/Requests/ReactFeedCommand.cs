using LiftNet.Contract.Enums.Feed;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class ReactFeedCommand : IRequest<LiftNetRes>
    {
        public string FeedId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ReactType Type { get; set; }
    }
} 
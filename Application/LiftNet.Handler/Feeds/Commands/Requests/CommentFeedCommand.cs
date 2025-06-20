using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Commands.Requests
{
    public class CommentFeedCommand : IRequest<LiftNetRes>
    {
        public string CallerId
        {
            get; set;
        }

        public string FeedId
        {
            get; set;
        }

        public string Comment
        {
            get; set;
        }

        public string? ParentId
        {
            get; set;
        }
    }
}

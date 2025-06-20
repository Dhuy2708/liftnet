using LiftNet.Contract.Views.Feeds;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Feeds.Queries.Requests
{
    public class ListCommentsQuery : IRequest<LiftNetRes<CommentView>>  
    {
        public string FeedId
        {
            get; set;
        }
        public string? ParentId
        {
            get; set;
        }
    }
}

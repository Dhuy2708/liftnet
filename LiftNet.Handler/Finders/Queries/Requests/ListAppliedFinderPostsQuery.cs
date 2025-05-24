using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class ListAppliedFinderPostsQuery : IRequest<LiftNetRes<ExploreFinderPostView>>
    {
        public string UserId
        {
            get; set;
        }
    }
}

using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class ExploreFinderPostsQuery : IRequest<LiftNetRes<List<FinderPostView>>>
    {
        public string UserId { get; set; }
    }
} 
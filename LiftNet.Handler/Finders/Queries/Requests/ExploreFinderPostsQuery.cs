using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class ExploreFinderPostsQuery : IRequest<LiftNetRes<ExploreFinderPostView>>
    {
        public string UserId { get; set; }
        public float MaxDistance { get; set; }
    }
} 
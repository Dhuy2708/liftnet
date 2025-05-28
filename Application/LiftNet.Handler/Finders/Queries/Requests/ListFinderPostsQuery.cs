using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class ListFinderPostsQuery : IRequest<PaginatedLiftNetRes<FinderPostView>>
    {
        public QueryCondition Conditions { get; set; }
        public string UserId { get; set; }
    }
} 
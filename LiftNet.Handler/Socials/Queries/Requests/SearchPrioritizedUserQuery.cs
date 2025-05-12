using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Socials.Queries.Requests
{
    public class SearchPrioritizedUserQuery : IRequest<PaginatedLiftNetRes<UserOverview>>
    {
        public string UserId { get; set; }
        public QueryCondition Conditions { get; set; }
    }
} 
using LiftNet.Contract.Views.Plannings;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Plannings.Queries.Requests
{
    public class GetPhysicalStatsQuery : IRequest<LiftNetRes<UserPhysicalStatView>>
    {
        public string UserId { get; set; }
    }
} 
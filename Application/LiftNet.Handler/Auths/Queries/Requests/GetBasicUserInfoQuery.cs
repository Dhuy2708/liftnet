using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Auths.Queries.Requests
{
    public class GetBasicUserInfoQuery : IRequest<LiftNetRes<BasicUserInfo>>
    {
        public string UserId { get; set; }
    }
} 
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using MediatR;

namespace LiftNet.Handler.Admins.Queries.Requests
{
    public class ListUsersQuery : IRequest<LiftNetRes<UserItemView>>
    {
        public string CallerId { get; set; }
    }
} 
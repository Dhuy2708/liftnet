using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Queries.Requests
{
    public class FriendSuggestionQuery : IRequest<LiftNetRes<UserOverview>>
    {
        public string UserId
        {
            get; set;
        }
    }
}

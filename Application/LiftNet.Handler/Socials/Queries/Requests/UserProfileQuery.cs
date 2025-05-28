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
    public class UserProfileQuery : IRequest<LiftNetRes<ProfileView>>
    {
        public bool IsSelf
        {
            get; set;
        }
        public string UserId
        {
            get; set;
        }
        public string ProfileId
        {
            get; set;
        }
    }
}

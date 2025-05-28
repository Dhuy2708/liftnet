using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Commands.Requets
{
    public class UnfollowUserCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public string TargetId
        {
            get; set;
        }
    }
}

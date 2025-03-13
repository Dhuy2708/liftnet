using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auths.Commands.Requests
{
    public class LoginCommand : IRequest<LiftNetRes<string>>
    {
        public string Username
        {
            get; set;
        }
        public string Password
        {
            get; set;
        }
    }
}

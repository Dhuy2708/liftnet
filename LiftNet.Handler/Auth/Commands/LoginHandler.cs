using LiftNet.Domain.Response;
using LiftNet.Handler.Auth.Commands.Requests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auth.Commands
{
    public class LoginHandler : IRequestHandler<LoginCommand, LiftNetRes<string>>
    {
        public Task<LiftNetRes<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

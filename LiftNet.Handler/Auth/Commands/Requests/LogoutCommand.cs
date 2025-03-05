using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auth.Commands.Requests
{
    public class LogoutCommand : IRequest<LiftNetRes>
    {
    }
}

using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Socials.Commands.Requets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Commands
{
    public class FollowUserHandler : IRequestHandler<FollowUserCommand, LiftNetRes>
    {
        private readonly ILiftLogger<FollowUserHandler> _logger;
        public Task<LiftNetRes> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
} 

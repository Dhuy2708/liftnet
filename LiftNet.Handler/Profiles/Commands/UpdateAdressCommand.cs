using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Profiles.Commands
{
    public class UpdateAdressCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public string LocationId
        {
            get; set;
        }
    }
}

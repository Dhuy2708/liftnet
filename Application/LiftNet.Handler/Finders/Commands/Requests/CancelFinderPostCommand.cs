using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands.Requests
{
    public class CancelFinderPostCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public string PostId
        {
            get; set;
        }
        public string Reason
        {
            get; set;
        }
    }
}

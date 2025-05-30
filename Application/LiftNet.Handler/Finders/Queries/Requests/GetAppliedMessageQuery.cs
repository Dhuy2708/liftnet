using LiftNet.Contract.Views.Finders;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Queries.Requests
{
    public class GetAppliedMessageQuery : IRequest<LiftNetRes<AppliedFinderPostMessage>>
    {
        public string UserId
        {
            get; set;
        }
        public string PostId
        {
            get; set;
        }
    }
}

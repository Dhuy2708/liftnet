using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands.Requests
{
    public class RecommendSeekerToPtCommand : IRequest<LiftNetRes>
    {
        public string SeekerId
        {
            get; set;
        }

        public List<string> PTIds
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }
    }
}

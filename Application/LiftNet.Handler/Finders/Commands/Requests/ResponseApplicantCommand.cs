using LiftNet.Contract.Enums.Finder;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands.Requests
{
    public class ResponseApplicantCommand : IRequest<LiftNetRes>
    {
        public string UserId
        {
            get; set;
        }
        public int ApplicantId
        {
            get; set;
        }
        public FinderPostResponseType Status
        {
            get; set;
        }
    }
}

using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.SideBars.Queries.Requests
{
    public class GetUnreadCountQuery : IRequest<LiftNetRes<Dictionary<string, int>>>
    {
        public string UserId
        {
            get; set;
        }
    }
}

using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Socials.Queries.Requests
{
    public class ListFollowingRequest : IRequest<PaginatedLiftNetRes<UserOverview>>
    {
        public string CallerId
        {
            get; set;
        }

        public QueryCondition Conditions
        {
            get; set;
        }
    }
}

using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Searches.Queries.Requests
{
    public class SearchUserRequest : IRequest<PaginatedLiftNetRes<UserOverview>>
    {
        public string UserId
        {
            get; set;
        }   
        public QueryCondition Conditions
        {
            get; set;
        }
    }
}

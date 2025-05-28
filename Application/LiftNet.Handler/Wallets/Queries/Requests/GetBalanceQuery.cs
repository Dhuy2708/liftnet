using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Wallets.Queries.Requests
{
    public class GetBalanceQuery : IRequest<LiftNetRes<double>>
    {
        public string UserId
        {
            get; set;
        }
    }
}

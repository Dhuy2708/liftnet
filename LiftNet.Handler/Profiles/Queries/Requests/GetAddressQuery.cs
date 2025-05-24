using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Profiles.Queries.Requests
{
    public class GetAddressQuery : IRequest<LiftNetRes<AddressView>>
    {
        public string UserId
        {
            get; set;
        }
    }
}

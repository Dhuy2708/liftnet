using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Geos.Queries
{
    public class SearchProvincesRequest : IRequest<LiftNetRes<List<ProvinceView>>>
    {
        public string Q
        {
            get; set;
        }
    }
}

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
    public class SearchWardsRequest : IRequest<LiftNetRes<List<WardView>>>
    {
        public int ProvinceCode
        {
            get; set;
        }
        public int DistrictCode
        {
            get; set;
        }
        public string Q
        {
            get; set;
        }
    }
}

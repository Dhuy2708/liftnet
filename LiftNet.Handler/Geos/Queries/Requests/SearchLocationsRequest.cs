using LiftNet.Domain.Response;
using MediatR;
using LiftNet.Contract.Views;

namespace LiftNet.Handler.Geos.Queries.Requests
{
    public class SearchLocationsRequest : IRequest<LiftNetRes<List<PlacePredictionView>>>
    {
        public int ProvinceCode { get; set; }
        public int DistrictCode { get; set; }
        public int WardCode { get; set; }
        public string Input { get; set; }
    }
}
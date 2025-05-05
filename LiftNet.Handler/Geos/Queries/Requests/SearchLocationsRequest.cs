using LiftNet.Domain.Response;
using MediatR;
using LiftNet.Contract.Views;

namespace LiftNet.Handler.Geos.Queries.Requests
{
    public class SearchLocationsRequest : IRequest<LiftNetRes<PlacePredictionView>>
    {
        public String UserId
        {
            get; set;
        }
        public string Input
        {
            get; set;
        }
        public bool SearchRelated
        {
            get; set;
        }
    }
}
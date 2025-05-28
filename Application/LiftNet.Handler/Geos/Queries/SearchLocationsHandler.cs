using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using LiftNet.Handler.Geos.Queries.Requests;
using MediatR;

namespace LiftNet.Handler.Geos.Queries
{
    public class SearchLocationsHandler : IRequestHandler<SearchLocationsRequest, LiftNetRes<PlacePredictionView>>
    {
        private readonly IGeoService _geoService;
        private readonly IAddressIndexService _addressService;

        public SearchLocationsHandler(IGeoService geoService, IAddressIndexService addressService)
        {
            _geoService = geoService;
            _addressService = addressService;
        }

        public async Task<LiftNetRes<PlacePredictionView>> Handle(SearchLocationsRequest request, CancellationToken cancellationToken)
        {
            double? latitude = null;
            double? longitude = null;

            if (request.SearchRelated)
            {
                var userAddress = await _addressService.GetAsync(request.UserId, request.UserId);
                if (userAddress != null)
                {
                    latitude = userAddress.Location.Latitude;
                    longitude = userAddress.Location.Longitude;
                }
            }

            var predictions = await _geoService.AutocompleteSearchAsync(request.Input, latitude, longitude);

            return LiftNetRes<PlacePredictionView>.SuccessResponse(predictions);
        }
    }
}
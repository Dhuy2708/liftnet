using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using LiftNet.Handler.Geos.Queries.Requests;
using MediatR;

namespace LiftNet.Handler.Geos.Queries
{
    public class SearchLocationsHandler : IRequestHandler<SearchLocationsRequest, LiftNetRes<List<PlacePredictionView>>>
    {
        private readonly IGeoService _geoService;

        public SearchLocationsHandler(IGeoService geoService)
        {
            _geoService = geoService;
        }

        public async Task<LiftNetRes<List<PlacePredictionView>>> Handle(SearchLocationsRequest request, CancellationToken cancellationToken)
        {
            double? latitude = null;
            double? longitude = null;

            if (request.ProvinceCode > 0)
            {
                var coordinates = await _geoService.GetCoordinatesByProvinceCodeAsync(request.ProvinceCode);
                latitude = coordinates.lat;
                longitude = coordinates.lng;    
            }

            var predictions = await _geoService.AutocompleteSearchAsync(request.Input, latitude, longitude);

            return LiftNetRes<List<PlacePredictionView>>.SuccessResponse(predictions);
        }
    }
}
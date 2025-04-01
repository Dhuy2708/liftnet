using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Geos.Queries;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Geos
{
    public class SearchDistrictsHandler : IRequestHandler<SearchDistrictsRequest, LiftNetRes<List<DistrictView>>>
    {
        private readonly ILiftLogger<SearchDistrictsHandler> _logger;
        private readonly IGeoService _geoService;

        public SearchDistrictsHandler(ILiftLogger<SearchDistrictsHandler> logger, IGeoService geoService)
        {
            _logger = logger;
            _geoService = geoService;
        }

        public async Task<LiftNetRes<List<DistrictView>>> Handle(SearchDistrictsRequest request, CancellationToken cancellationToken)
        {
            _logger.Info("begin search districts");

            var dtos = await _geoService.SearchDistrictsAsync(request.ProvinceCode, request.Q);
            if (dtos == null)
            {
                return LiftNetRes<List<DistrictView>>.ErrorResponse("Failed to search districts");
            }
            var views = dtos.Select(x => x.ToView()).ToList();
            return LiftNetRes<List<DistrictView>>.SuccessResponse(views);
        }
    }
}

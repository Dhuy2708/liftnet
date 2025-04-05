using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Geos.Queries;
using LiftNet.Utility.Mappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Geos
{
    public class SearchWardsHandler : IRequestHandler<SearchWardsRequest, LiftNetRes<List<WardView>>>
    {
        private readonly ILiftLogger<SearchWardsHandler> _logger;
        private readonly IGeoService _geoService;

        public SearchWardsHandler(ILiftLogger<SearchWardsHandler> logger, IGeoService geoService)
        {
            _logger = logger;
            _geoService = geoService;
        }

        public async Task<LiftNetRes<List<WardView>>> Handle(SearchWardsRequest request, CancellationToken cancellationToken)
        {
            _logger.Info("begin search wards");

            var dtos = await _geoService.SearchWardsAsync(request.ProvinceCode, request.DistrictCode, request.Q);

            if (dtos == null || !dtos.Any())
            {
                return LiftNetRes<List<WardView>>.ErrorResponse("No wards found");
            }
            var views = dtos.Select(x => x.ToView()).ToList();
            return LiftNetRes<List<WardView>>.SuccessResponse(views);
        }
    }
}

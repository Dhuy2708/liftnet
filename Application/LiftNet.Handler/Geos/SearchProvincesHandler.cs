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
    public class SearchProvincesHandler : IRequestHandler<SearchProvincesRequest, LiftNetRes<List<ProvinceView>>>
    {
        private readonly ILiftLogger<SearchProvincesRequest> _logger;
        private IGeoService _geoService;

        public SearchProvincesHandler(ILiftLogger<SearchProvincesRequest> logger, IGeoService geoService)
        {
            _logger = logger;
            _geoService = geoService;
        }

        public async Task<LiftNetRes<List<ProvinceView>>> Handle(SearchProvincesRequest request, CancellationToken cancellationToken)
        {
            _logger.Info("begin search provinces");

            var dtos = await _geoService.SearchProvincesAsync(request.Q);
            
            if (dtos == null || !dtos.Any())
            {
                return LiftNetRes<List<ProvinceView>>.ErrorResponse("No provinces found");
            }
            var views = dtos.Select(x => x.ToView()).ToList();
            return LiftNetRes<List<ProvinceView>>.SuccessResponse(views);
        }
    }
}

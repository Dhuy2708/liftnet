using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
using LiftNet.Handler.Geos.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class GeoController : LiftNetControllerBase
    {
        public GeoController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("provinces/search")]
        [ProducesResponseType(typeof(List<ProvinceView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProvinces([FromQuery] string q)
        {
            var req = new SearchProvincesRequest()
            {
                Q = q
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("districts/search")]
        [ProducesResponseType(typeof(List<DistrictView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProvinces([FromQuery] int provinceCode, [FromQuery] string q)
        {
            var req = new SearchDistrictsRequest()
            {
                ProvinceCode = provinceCode,
                Q = q
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("wards/search")]
        [ProducesResponseType(typeof(List<WardView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProvinces([FromQuery] int provinceCode, [FromQuery] int districtCode, [FromQuery] string q)
        {
            var req = new SearchWardsRequest()
            {
                ProvinceCode = provinceCode,
                DistrictCode = districtCode,
                Q = q
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

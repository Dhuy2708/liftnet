using LiftNet.Contract.Dtos;
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
        [ProducesResponseType(typeof(ProvinceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProvinces([FromQuery] string q)
        {
            
        }
    }
}

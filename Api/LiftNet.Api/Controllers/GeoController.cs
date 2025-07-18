﻿using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using LiftNet.Handler.Geos.Queries;
using LiftNet.Handler.Geos.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(typeof(LiftNetRes<ProvinceView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProvinces([FromQuery] string q)
        {
            var req = new SearchProvincesRequest()
            {
                Q = q ?? string.Empty
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("districts/search")]
        [ProducesResponseType(typeof(LiftNetRes<DistrictView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchProvinces([FromQuery] int provinceCode, [FromQuery] string q)
        {
            var req = new SearchDistrictsRequest()
            {
                ProvinceCode = provinceCode,
                Q = q ?? string.Empty
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
                Q = q ?? string.Empty
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("location/search")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<PlacePredictionView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchLocations([FromQuery] string q, [FromQuery] bool searchRelated = false)
        {
            var req = new SearchLocationsRequest()
            {
                UserId = UserId,
                Input = q,
                SearchRelated = searchRelated
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

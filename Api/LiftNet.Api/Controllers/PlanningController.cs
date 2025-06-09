using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views.Users;
using LiftNet.Handler.Plannings.Commands.Requests;
using LiftNet.Handler.Plannings.Queries.Requests;
using LiftNet.Api.Requests.Plannings;
using LiftNet.Utility.Extensions;
using LiftNet.Contract.Views.Plannings;

namespace LiftNet.Api.Controllers
{
    public class PlanningController : LiftNetControllerBase
    {
        public PlanningController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("setPhysicalStat")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetPhysicalStats([FromBody] SetPhysicalStatsReq req)
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            var command = new SetPhysicalStatsCommand
            {
                UserId = UserId,
                Age = req.Age,
                Gender = req.Gender,
                Height = req.Height,
                Mass = req.Mass,
                Bdf = req.Bdf,
                ActivityLevel = (int?)req.ActivityLevel,
                Goal = (int?)req.Goal
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("getPhysicalStat")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<UserPhysicalStatView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPhysicalStats()
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            var query = new GetPhysicalStatsQuery
            {
                UserId = UserId
            };

            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

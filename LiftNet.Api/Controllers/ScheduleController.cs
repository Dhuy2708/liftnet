using LiftNet.Api.Requests.Schedules;
using LiftNet.Contract.Views.Schedules;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.Schedules.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class ScheduleController : LiftNetControllerBase
    {
        public ScheduleController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("events")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<EventView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListEvents([FromQuery] ListEventRequest req)
        {
            var queryReq = new ListEventQueryRequest()
            {
                UserId = UserId,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                EventSearch = req.EventSearch,
                UserIds = req.UserIds
            };
            var result = await _mediator.Send(queryReq);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

using LiftNet.Api.Requests.Appointments;
using LiftNet.Domain.Response;
using LiftNet.Handler.SideBars.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class SideBarController : LiftNetControllerBase
    {
        public SideBarController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("unreadCount")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<Dictionary<string, int>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUnreadCOunt()
        {
            var request = new GetUnreadCountQuery()
            {
                UserId = UserId
            };
            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

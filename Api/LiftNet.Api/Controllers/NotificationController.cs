using LiftNet.Contract.Views.Notis;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Queries.Requests;
using LiftNet.Handler.Notis.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class NotificationController : LiftNetControllerBase
    {
        public NotificationController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpGet("list")]
        [Authorize]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<NotificationView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var req = new ListNotificationsQuery()
            {
                CallerId = UserId,
                PageNumber = pageNumber,
                PageSize = pageSize
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

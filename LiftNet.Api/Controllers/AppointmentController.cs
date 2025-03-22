
using LiftNet.Api.Requests;
using LiftNet.Api.ToDto;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Handler.Appointments.Queries.Requests;
using LiftNet.Utility.Mappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class AppointmentController : LiftNetControllerBase
    {
        public AppointmentController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("book")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> BookAppointment(BookAppointmentReq req)
        {
            var request = new BookAppointmentCommand()
            {
                Appointment = req.ToDto(),
            };
            var result = await _mediator.Send(request);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("list")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<AppointmentView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListAppointments([FromQuery] QueryCondition cond)
        {
            var req = new ListAppointmentsQuery()
            {
                UserId = UserId,
                Conditions = cond,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }


        [HttpGet("{id}")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<AppointmentDetailView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAppointment([FromRoute] string id)
        {
            var req = new GetAppointmentQuery()
            {
                Id = id,
                UserId = UserId,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("actionRequest")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AppointmentActionRequest(AppointmentActionReq req)
        {
            var request = new AppointmentActionCommand()
            {
                UserId = UserId,
                AppointmentId = req.AppointmentId,
                Action = req.Action,
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

using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LiftNet.Api.Requests.Auths;
using LiftNet.Handler.Admins.Commands.Requests;
using LiftNet.Handler.Admins.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Contract.Views.Users;

namespace LiftNet.Api.Controllers
{
    [Authorize(Policy = LiftNetPolicies.Admin)]
    public class AdminController : LiftNetControllerBase
    {
        public AdminController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("user/create")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUser([FromBody] RegisterRequest req)
        {
            var command = new CreateUserCommand
            {
                CallerId = UserId,
                Email = req.Email,
                Username = req.Username,
                Password = req.Password,
                FirstName = req.FirstName,
                LastName = req.LastName,
                Age = req.Age,
                Gender = req.Gender,
                Role = req.Role
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(LiftNetRes<UserItemView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListUsers()
        {
            var query = new ListUsersQuery
            {
                CallerId = UserId
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

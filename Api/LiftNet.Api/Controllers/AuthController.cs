using LiftNet.Api.Requests.Auths;
using LiftNet.Api.ToDto;
using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Linq;
using System.Security.Claims;
using LiftNet.Domain.Constants;
using LiftNet.Handler.Auths.Queries.Requests;
using LiftNet.Utility.Extensions;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : LiftNetControllerBase
    {

        public AuthController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var result = await _mediator.Send(req.ToCommand());
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LiftNetRes<TokenInfo>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _mediator.Send(req.ToCommand());
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("admin/login")]
        [ProducesResponseType(typeof(LiftNetRes<TokenInfo>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AdminLogin(LoginRequest req)
        {
            var command = new AdminLoginCommand
            {
                Username = req.Email,
                Password = req.Password
            };
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand());
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("basicInfo")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<BasicUserInfo>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBasicInfo()
        {
            var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(LiftNetRes.ErrorResponse("User not authenticated"));
            }

            var query = new GetBasicUserInfoQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpGet("check")]
        [Authorize]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult Check()
        {
            return Ok(UserId.IsNotNullOrEmpty());
        }
    }
}

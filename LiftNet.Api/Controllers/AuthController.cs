using LiftNet.Api.Requests;
using LiftNet.Api.ToDto;
using LiftNet.Contract.Views;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPost("logout")]
        [Authorize]
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
    }
}

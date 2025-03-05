using LiftNet.Api.Contracts.Requests;
using LiftNet.Api.ToDto;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auth.Commands.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : LiftNetControllerBase
    {
        private readonly IMediator _mediator;
        
        public AuthController(IMediator mediator, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mediator = mediator;
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
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
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

using LiftNet.Api.Contracts.Requests;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
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
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        
        public AuthController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(LiftNetRes<string>), (int)HttpStatusCode.OK))]
        public async Task<IActionResult> Register(RegisterRequest model)
        {

            return Ok();
        }

    }
}

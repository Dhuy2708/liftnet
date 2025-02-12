using LiftNet.Api.Contracts.Requests;
using LiftNet.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : LiftNetControllerBase
    {
        public AuthController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {

        }

    }
}

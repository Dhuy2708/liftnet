using LiftNet.Domain.Response;
using LiftNet.Handler.Wallets.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class WalletController : LiftNetControllerBase
    {
        public WalletController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }


        [HttpGet("getBalance")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<double>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBalance()
        {
            if (UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }
            var request = new GetBalanceQuery
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

        [HttpGet("paymentHistories")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes<double>), (int)HttpStatusCode.OK)]

    }
}

using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Response;
using LiftNet.Handler.Searches.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class SearchController : LiftNetControllerBase
    {
        public SearchController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("users")]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<UserOverview>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchUser(QueryCondition cond)
        {
            var req = new SearchUserRequest()
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
    }
}

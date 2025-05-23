using LiftNet.Api.Requests.Profiles;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using LiftNet.Handler.Profiles.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LiftNet.Api.Controllers
{
    public class ProfileController : LiftNetControllerBase
    {
        public ProfileController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("upload/avatar")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            var req = new UploadAvatarCommand()
            {
                UserId = UserId,
                File = file,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("upload/cover")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadCover([FromForm] IFormFile file)
        {
            var req = new UploadCoverCommand()
            {
                UserId = UserId,
                File = file,
            };
            var result = await _mediator.Send(req);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("update")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
        {
            var command = new UpdateProfileCommand()
            {
                UserId = UserId,
                Age = req.Age,
                Bio = req.Bio,
                FirstName = req.FirstName,
                LastName = req.LastName,
                Gender = req.Gender,
                Location = req.Location,
            };
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("update/address")]
        [Authorize]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAddress([FromBody] string locationId)
        {
            var command = new UpdateAdressCommand
            {
                UserId = UserId,
                LocationId = locationId
            };
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}

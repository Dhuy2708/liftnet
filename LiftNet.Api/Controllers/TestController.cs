using LiftNet.RedisCache.Interface;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LiftNet.Domain.Response;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : LiftNetControllerBase
    {
        private IRedisCacheService redisCacheService => _serviceProvider.GetRequiredService<IRedisCacheService>();
        public TestController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("cache/set")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetCache([FromBody] CacheRequest request)
        {
            if (request.Expiration.HasValue)
            {
                await redisCacheService.SetAsync(request.Key, request.Value, request.Expiration.Value);
            }
            else
            {
                await redisCacheService.SetAsync(request.Key, request.Value);
            }
            return Ok(LiftNetRes.SuccessResponse("Cache set successfully"));
        }

        [HttpGet("cache/get/{key}")]
        [ProducesResponseType(typeof(LiftNetRes<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCache([FromRoute] string key)
        {
            var value = await redisCacheService.GetObjectAsync<string>(key);
            if (value == null)
            {
                return NotFound(LiftNetRes<string>.ErrorResponse("Key not found"));
            }
            return Ok(LiftNetRes<string>.SuccessResponse(value));
        }

        [HttpDelete("cache/delete/{key}")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteCache([FromRoute] string key)
        {
            await redisCacheService.RemoveAsync(key);
            return Ok(LiftNetRes.SuccessResponse("Cache deleted successfully"));
        }

        [HttpGet("cache/exists/{key}")]
        [ProducesResponseType(typeof(LiftNetRes<bool>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CheckCacheExists([FromRoute] string key)
        {
            var exists = await redisCacheService.ExistsAsync(key);
            return Ok(LiftNetRes<bool>.SuccessResponse(exists));
        }

        [HttpPost("cache/expire/{key}")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetCacheExpiration([FromRoute] string key, [FromBody] ExpirationRequest request)
        {
            var success = await redisCacheService.SetExpirationAsync(key, request.Expiration);
            if (!success)
            {
                return NotFound(LiftNetRes.ErrorResponse("Key not found"));
            }
            return Ok(LiftNetRes.SuccessResponse("Expiration set successfully"));
        }
    }

    public class CacheRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public TimeSpan? Expiration { get; set; }
    }

    public class ExpirationRequest
    {
        public TimeSpan Expiration { get; set; }
    }
}

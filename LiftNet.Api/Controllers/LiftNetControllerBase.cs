using LiftNet.Domain.Constants;
using LiftNet.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiftNetControllerBase : ControllerBase
    {
        /// <summary>
        /// credential
        /// </summary>
        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        private readonly IServiceProvider _serviceProvider;
        protected readonly IMediator _mediator;

        private ILiftLogger<LiftNetControllerBase> Logger => _serviceProvider.GetService<ILiftLogger<LiftNetControllerBase>>()!;

        public LiftNetControllerBase(IMediator mediator, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _mediator = mediator;
        }

        protected void LogInfo(string message)
        {
            Logger.Info(message);
        }

        protected void LogWarning(string message)
        {
            Logger.Warn(message);
        }

        protected void LogError(string message, Exception exception)
        {
            Logger.Error(exception, message);
        }
    }
}

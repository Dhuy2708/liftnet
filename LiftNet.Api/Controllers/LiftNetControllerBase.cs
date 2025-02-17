using LiftNet.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LiftNet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiftNetControllerBase : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ILiftLogger<LiftNetControllerBase> Logger => _serviceProvider.GetService<ILiftLogger<LiftNetControllerBase>>()!;

        public LiftNetControllerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected void LogInfo(string message)
        {
            Logger.LogInformation(message);
        }

        protected void LogWarning(string message)
        {
            Logger.LogWarning(message);
        }

        protected void LogError(string message, Exception exception)
        {
            Logger.LogError(exception, message);
        }
    }
}

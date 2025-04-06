using LiftNet.Domain.Constants;
using LiftNet.Domain.Enums;
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
        protected string UserId => User?.FindFirstValue(LiftNetClaimType.UId) ?? "";
        protected string Username => User?.FindFirstValue(LiftNetClaimType.Username) ?? "";
        
        protected LiftNetRoleEnum Role => Roles?.FirstOrDefault() ?? LiftNetRoleEnum.None;
        
        protected List<LiftNetRoleEnum> Roles
        {
            get
            {
                var rolesString = User?.FindFirstValue(LiftNetClaimType.Roles);
                if (string.IsNullOrEmpty(rolesString))
                    return [];

                return rolesString.Split(", ", StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => 
                                {
                                    return Enum.TryParse<LiftNetRoleEnum>(x, out var role) 
                                        ? role 
                                        : LiftNetRoleEnum.None;
                                })
                                .ToList();
            }
        }

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

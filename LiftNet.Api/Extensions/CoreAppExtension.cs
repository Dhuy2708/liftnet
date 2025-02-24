using LiftNet.Domain.Response;
using LiftNet.Handler.Auth.Commands.Requests;
using LiftNet.Handler.Auth.Commands;
using MediatR;
using LiftNet.Domain.Interfaces;
using LiftNet.Logger.Core;
using Microsoft.Extensions.Logging;

namespace LiftNet.Api.Extensions
{
    public static class CoreAppExtension
    {
        public static IServiceCollection RegisterCqrs(this IServiceCollection services)
        {
            #region cqrs
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                                    typeof(Handler.HandlerAssemblyRef).Assembly,
                                    typeof(SharedKenel.CoreCQRSAssemblyRef).Assembly
                                ));

            // handler
            services.AddScoped<IRequestHandler<RegisterCommand, LiftNetRes>, RegisterCommandHandler>();
            #endregion

            #region logger
            services.AddScoped(typeof(ILiftLogger<>), typeof(LiftLogger<>));
            #endregion

            #region inmemory
            services.AddMemoryCache();
            #endregion
            return services;
        }

        public static IServiceCollection RegisterAppContext(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services;
        }
    }
}

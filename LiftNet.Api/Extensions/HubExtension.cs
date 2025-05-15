using LiftNet.Hub;
using LiftNet.Hub.Core;
using LiftNet.Hub.Provider;
using Microsoft.AspNetCore.SignalR;

namespace LiftNet.Api.Extensions
{
    public static class HubExtension
    {
        public static IServiceCollection RegisterHubs(this IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, LiftNetUserIdProvider>();
            var enableDetailedErrors = false;
#if DEBUG
            enableDetailedErrors = true;
#endif

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = enableDetailedErrors;
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            });

            services.AddSingleton<ConnectionPool>();
            services.AddSingleton<ChatHub>();
            return services;
        }
    }
}

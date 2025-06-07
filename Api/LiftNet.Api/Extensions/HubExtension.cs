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

            services.AddSignalR();

            services.AddSingleton<ConnectionPool>();
            services.AddTransient<ChatHub>();
            services.AddSingleton<NotiHub>();
            return services;
        }
    }
}

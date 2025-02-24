using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Repositories;

namespace LiftNet.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepo, AuthRepo>();

            return services;
        }
    }
}

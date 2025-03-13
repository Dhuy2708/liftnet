using LiftNet.Domain.Interfaces;
using LiftNet.Ioc;
using LiftNet.Logger.Core;
using System.Reflection;

namespace LiftNet.Api.Extensions
{
    public static class CoreAppExtension
    {
        public static IServiceCollection RegisterCqrs(this IServiceCollection services)
        {
            #region ioc
            services.AddDependencies(typeof(Handler.HandlerAssemblyRef).Assembly);
            services.AddDependencies(typeof(Repositories.RepoAssemblyRef).Assembly);
            #endregion

            #region cqrs
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                                    typeof(Handler.HandlerAssemblyRef).Assembly,
                                    typeof(SharedKenel.CoreCQRSAssemblyRef).Assembly
                                ));
            #endregion

            #region logger
            services.AddSingleton(typeof(ILiftLogger<>), typeof(LiftLogger<>));
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

        private static IServiceCollection AddDependencies(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IDependency).IsAssignableFrom(t));

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(i => i != typeof(IDependency));
                foreach (var @interface in interfaces)
                {
                    services.AddScoped(@interface, type);
                }
            }

            return services;
        }
    }
}

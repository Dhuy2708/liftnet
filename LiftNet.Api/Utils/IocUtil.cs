using LiftNet.Ioc;
using System.Reflection;

namespace LiftNet.Api.Utils
{
    public static class IocUtil
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, Assembly assembly)
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

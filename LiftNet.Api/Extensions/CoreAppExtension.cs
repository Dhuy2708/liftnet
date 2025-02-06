namespace LiftNet.Api.Extensions
{
    public static class CoreAppExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            // cqrs
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                                    typeof(Command.CommandAssemblyRef).Assembly,
                                    typeof(Query.QueryAssemblyRef).Assembly,
                                    typeof(SharedKenel.CoreCQRSAssemblyRef).Assembly
                                ));


            return services;
        }
    }
}

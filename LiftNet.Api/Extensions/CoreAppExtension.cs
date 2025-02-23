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
            #endregion

            #region http context
            services.AddHttpContextAccessor();
            #endregion
            return services;


        }
    }
}

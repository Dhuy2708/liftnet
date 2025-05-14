using LiftNet.Engine.Engine;
using LiftNet.Engine.Engine.Impl;

namespace LiftNet.Api.Extensions
{
    public static class EngineServiceExtension
    {
        public static IServiceCollection RegisterEngines(this IServiceCollection services)
        {
            services.RegisterSocialEngine();
            services.RegisterFeedEngine();
            return services;
        }   

        private static IServiceCollection RegisterSocialEngine(this IServiceCollection services)
        {
            services.AddSingleton<ISocialEngine, SocialEngine>(); 
            return services;
        }

        private static IServiceCollection RegisterFeedEngine(this IServiceCollection services)
        {
            services.AddSingleton<IFeedEngine, FeedEngine>();
            return services;
        }
    }
}

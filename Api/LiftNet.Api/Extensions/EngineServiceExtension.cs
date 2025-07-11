﻿using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IServices;
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
            services.RegisterChatbotEngine();
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

        private static IServiceCollection RegisterChatbotEngine(this IServiceCollection services)
        {
            var engineUrl = Environment.GetEnvironmentVariable(EnvKeys.LOCAL_CHATBOT_ENGINE_URL);
#if !DEBUG
                engineUrl = Environment.GetEnvironmentVariable(EnvKeys.CHATBOT_ENGINE_URL);
#endif
            if (string.IsNullOrEmpty(engineUrl))
            {
                throw new ArgumentNullException("Chatbot engine URL not found.");
            }

            services.AddSingleton(new ChatBotUrl
            {
                Url = engineUrl
            });
            services.AddScoped<IChatBotEngine, ChatbotEngine>(x =>
            {
              
                return new ChatbotEngine(engineUrl);
            });
            return services;
        }
    }
}

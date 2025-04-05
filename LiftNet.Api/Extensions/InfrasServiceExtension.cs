using System.Diagnostics;
using dotenv.net;
using LiftNet.Api.Utils;
using LiftNet.AzureBlob.Services;
using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.CosmosDb.Services;
using LiftNet.MapSDK.Apis;
using LiftNet.MapSDK.Contracts;
using LiftNet.ProvinceSDK.Apis;
using LiftNet.Timer.Service;
using LiftNet.Utility.Extensions;
using Microsoft.Azure.Cosmos;
using Quartz;

namespace LiftNet.Api.Extensions
{
    public static class InfrasServiceExtension
    {
        public static IServiceCollection RegisterInfras(this IServiceCollection services)
        {
            DotEnv.Load();

            #region blob
            var blobCnnStr = Environment.GetEnvironmentVariable(EnvKeys.BLOB_CONNECTION_STRING)!;
            services.AddSingleton<IBlobService>(provider =>
                                    new BlobService(provider.GetRequiredService<ILogger<BlobService>>(), blobCnnStr));
            #endregion

            #region cosmos
            var cosmosCnnStr = Environment.GetEnvironmentVariable(EnvKeys.COSMOS_CONNECTION_STRING) ?? throw new ArgumentNullException("COSMOS_CONNECTION_STRING is null.");
            var cosmosDbId = Environment.GetEnvironmentVariable(EnvKeys.COSMOS_DATABASE_ID) ?? throw new ArgumentNullException("COSMOS_DATABASE_ID is null");

            var cosmosClient = new CosmosClient(cosmosCnnStr);
            services.AddSingleton(provider => new CosmosCredential(cosmosClient, cosmosDbId));
     
            services.AddScoped(typeof(IIndexBaseService<>), typeof(IndexBaseService<>));
            services.AddDependencies(typeof(CosmosDb.CosmosDbAssemblyRef).Assembly);
            #endregion

            #region mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            #region sdk
            // province
            services.AddDependencies(typeof(ProvinceSDK.ProvinceSdkAssemblyRef).Assembly);

            // map
            services.AddSingleton(new MapApiKey
            {
                Key = Environment.GetEnvironmentVariable(EnvKeys.GOONG_MAP_API_KEY)!
            });
            
            services.AddDependencies(typeof(MapSDK.MapSdkAssemblyRef).Assembly);
            #endregion

            #region quartz
#if !DEBUG
            services.RegisterQuartzService();
#endif
            #endregion

            return services;
        }

        public static IServiceCollection RegisterQuartzService(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.AddJob<ProvinceDiscService>(opts => opts.WithIdentity("ProvinceDiscJob", "DiscJobs"));

                q.AddTrigger(opts => opts
                    .ForJob("ProvinceDiscJob", "DiscJobs")
                    .WithIdentity("ProvinceDiscTrigger", "DiscTriggers")
                    .StartNow() 
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInHours(24 * 7)
                        .RepeatForever()
                    )
                );
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddTransient<ProvinceDiscService>();


            return services;
        }
    }
}

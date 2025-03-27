using dotenv.net;
using LiftNet.Api.Utils;
using LiftNet.AzureBlob.Services;
using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Services;
using Microsoft.Azure.Cosmos;

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
            var cosmosCnnStr = Environment.GetEnvironmentVariable(EnvKeys.COSMOS_CONNECTION_STRING);
            services.AddSingleton(provider => new CosmosClient(cosmosCnnStr));
            services.AddScoped(typeof(IIndexBaseService<>), typeof(IndexBaseService<>));
            services.AddDependencies(typeof(CosmosDb.CosmosDbAssemblyRef).Assembly);
            #endregion

            #region mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            return services;
        }
    }
}

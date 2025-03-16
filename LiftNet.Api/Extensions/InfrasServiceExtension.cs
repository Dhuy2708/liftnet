using dotenv.net;
using LiftNet.AzureBlob.Services;
using LiftNet.Contract.Interfaces.IServices;

namespace LiftNet.Api.Extensions
{
    public static class InfrasServiceExtension
    {
        public static IServiceCollection RegisterInfras(this IServiceCollection services)
        {
            DotEnv.Load();
            var connectionString = Environment.GetEnvironmentVariable("BLOB_CONNECTION_STRING")!;

            services.AddSingleton<IBlobService>(provider =>
                                    new BlobService(provider.GetRequiredService<ILogger<BlobService>>(), connectionString));

            #region mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion
            return services;
        }
    }
}

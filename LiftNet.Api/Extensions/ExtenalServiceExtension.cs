using dotenv.net;
using LiftNet.AzureBlob.Services;
using LiftNet.Contract.Interfaces.Service;

namespace LiftNet.Api.Extensions
{
    public static class ExtenalServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            DotEnv.Load();
            var connectionString = Environment.GetEnvironmentVariable("BLOB_CONNECTION_STRING")!;

            services.AddSingleton<IBlobService>(provider =>
                                    new BlobService(provider.GetRequiredService<ILogger<BlobService>>(), connectionString));

            return services;
        }
    }
}

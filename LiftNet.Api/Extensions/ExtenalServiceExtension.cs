using LiftNet.AzureBlob.Services;
using LiftNet.Contract.Interfaces.Service;

namespace LiftNet.Api.Extensions
{
    public static class ExtenalServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            services.AddSingleton<IBlobService, BlobService>();

            return services;
        }
    }
}

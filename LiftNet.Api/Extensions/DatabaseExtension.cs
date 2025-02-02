using dotenv.net;

namespace LiftNet.Api.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            DotEnv.Load();
            var connectionSrting = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;

            services.AddDbContext<>
        }
    }
}


using dotenv.net;
using LiftNet.Api.Extensions;
using LiftNet.Api.Middlewares;
using LiftNet.Extensions;

namespace LiftNet.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.RegisterDbConfig();
        builder.Services.RegisterAppContext();
        builder.Services.RegisterCqrs();
        builder.Services.RegisterService();
        builder.Services.RegisterInfras();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        #region app
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        #if DEBUG
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/openapi/v1.json", "LiftNet API V1");
            });
        }
        #endif

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.Run();
        #endregion;
    }
}

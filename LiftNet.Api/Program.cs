
using dotenv.net;
using LiftNet.Api.Extensions;
using LiftNet.Api.Middlewares;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace LiftNet.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.RegisterAuth();
        builder.Services.RegisterDbConfig();
        builder.Services.RegisterAppContext();
        builder.Services.RegisterCommon();
        builder.Services.RegisterInfras();

        builder.Services.AddControllers();

        // allow nullable fields in request model
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApiDocument(options =>
        {
            options.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = OpenApiSecuritySchemeType.ApiKey,
                In = OpenApiSecurityApiKeyLocation.Header,
                Name = "Authorization",
                Scheme = "Bearer"
            });

            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        #region app
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        #if DEBUG
        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi(options =>
            {
                options.DocumentTitle = "LiftNet API Documentation";
            });
        }
        #endif

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.Run();
        #endregion;
    }
}

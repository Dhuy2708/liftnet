
using dotenv.net;
using LiftNet.Api.Extensions;
using LiftNet.Api.Middlewares;
using LiftNet.Hub.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace LiftNet.Api;

public class Program
{
    public static void Main(string[] args)
    {
        JsonWebTokenHandler.DefaultMapInboundClaims = false;

        var builder = WebApplication.CreateBuilder(args);
        //builder.WebHost.ConfigureKestrel(options =>
        //{
        //    options.ListenAnyIP(8080);
        //});

        builder.Services.RegisterAuth();
        builder.Services.RegisterDbConfig();
        builder.Services.RegisterAppContext();
        builder.Services.RegisterCommon();
        builder.Services.RegisterInfras();
        builder.Services.RegisterHostedService();
        builder.Services.RegisterEngines();
        builder.Services.RegisterHubs();

        builder.Services.AddControllers();
        builder.Services.AddSwagger();

        // allow nullable fields in request model
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        var orginUrl = "http://localhost:5173";
        var adminUrl = "http://localhost:3000";

#if !DEBUG
        orginUrl = Environment.GetEnvironmentVariable("UI_URL") ?? "http://localhost:5173";
        adminUrl = Environment.GetEnvironmentVariable("ADMIN_URL") ?? "http://localhost:3000";
#endif
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.WithOrigins(orginUrl!)
                      .WithOrigins(adminUrl!)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        #region app
        var app = builder.Build();
        app.UseCors("AllowAll");
        // Configure the HTTP request pipeline.

        app.UseOpenApi();
        app.UseSwaggerUi(options =>
        {
            options.DocumentTitle = "LiftNet API Documentation";
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.MapControllers();

        app.MapHub<ChatHub>("/chat-hub");

        app.Run();
        #endregion;
    }
}

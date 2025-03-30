
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
        builder.Services.AddSwagger();

        // allow nullable fields in request model
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

//#if DEBUG
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin() 
                          .AllowAnyMethod()   
                          .AllowAnyHeader(); 
                });
        });
//#endif

        #region app
        var app = builder.Build();

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

        app.Run();
        #endregion;
    }
}

using LiftNet.Api.Utils;
using LiftNet.Contract.Constants;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Interfaces;
using LiftNet.Logger.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;
using LiftNet.WorkerService.Worker;
using LiftNet.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using LiftNet.Domain.Entities;
using LiftNet.Engine;

namespace LiftNet.Api.Extensions
{
    public static class CoreAppExtension
    {
        public static IServiceCollection RegisterAuth(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 6;
            }).AddRoles<Role>()
              .AddEntityFrameworkStores<LiftNetDbContext>()
              .AddDefaultTokenProviders();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

                x.SaveToken = true;
#if DEBUG
                x.RequireHttpsMetadata = false;
#endif
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = Environment.GetEnvironmentVariable(EnvKeys.JWT_ISSUER),
                    ValidAudience = Environment.GetEnvironmentVariable(EnvKeys.JWT_AUDIENCE),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)),

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
                x.MapInboundClaims = false;
            });
            JsonWebTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthorization(options =>
            {
                options.AddPolicy(LiftNetPolicies.SeekerOrCoach, policy =>
                    policy.RequireRole(LiftNetRoles.Seeker, LiftNetRoles.Coach));
                options.AddPolicy(LiftNetPolicies.Seeker, policy =>
                    policy.RequireRole(LiftNetRoles.Seeker));
                options.AddPolicy(LiftNetPolicies.Coach, policy =>
                    policy.RequireRole(LiftNetRoles.Coach));
            });

            return services;
        }
        public static IServiceCollection RegisterCommon(this IServiceCollection services)
        {
            #region ioc
            services.AddDependencies(typeof(Repositories.RepoAssemblyRef).Assembly);
            services.AddDependencies(typeof(Service.ServiceAssemblyRef).Assembly);
            #endregion

            #region cqrs
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                                    typeof(Handler.HandlerAssemblyRef).Assembly,
                                    typeof(SharedKenel.CoreCQRSAssemblyRef).Assembly
                                ));
            #endregion

            services.AddSingleton(typeof(ILiftLogger<>), typeof(LiftLogger<>));
            services.AddMemoryCache();

            return services;
        }
        public static IServiceCollection RegisterAppContext(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services;
        }
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApiDocument(options =>
            {
                options.Title = "Liftnet Apis";
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
            return services;
        }
        public static IServiceCollection RegisterHostedService(this IServiceCollection services)
        {
            services.AddHostedService<MessageProcessingWorker>();
            return services;
        }
    }
}

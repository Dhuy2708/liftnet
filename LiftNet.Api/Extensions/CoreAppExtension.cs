using LiftNet.Api.Utils;
using LiftNet.Contract.Constants;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Interfaces;
using LiftNet.Logger.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LiftNet.Api.Extensions
{
    public static class CoreAppExtension
    {
        public static IServiceCollection RegisterAuth(this IServiceCollection services)
        {
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
                    RoleClaimType = LiftNetClaimType.Roles,
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(LiftNetPolicies.SeekerOrCoach, policy =>
                    policy.RequireRole(LiftNetRoles.Seeker, LiftNetRoles.Coach));
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
    }
}

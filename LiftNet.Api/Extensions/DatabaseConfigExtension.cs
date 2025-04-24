using dotenv.net;
using LiftNet.Contract.Constants;
using LiftNet.Domain.Entities;
using LiftNet.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LiftNet.Api.Extensions
{
    public static class DatabaseConfigExtension
    {
        public static IServiceCollection RegisterDbConfig(this IServiceCollection services)
        {
            DotEnv.Load();
            var connectionString = string.Empty;

            connectionString = Environment.GetEnvironmentVariable(EnvKeys.SQL_CONNECTION_STRING)!;
            services.AddDbContext<LiftNetDbContext>(opt =>
            {
                opt.UseSqlServer(connectionString);
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            });

            services.RegisterPolicy();

            return services;
        }

        private static IServiceCollection RegisterPolicy(this IServiceCollection services)
        {
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("StudentOrTeacher", policy =>
            //        policy.RequireRole(NarutoRoles.Teacher, NarutoRoles.Student));
            //});

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("TeacherOrAdmin", policy =>
            //        policy.RequireRole(NarutoRoles.Teacher, NarutoRoles.Admin));
            //});

            return services;
        }
    }
}

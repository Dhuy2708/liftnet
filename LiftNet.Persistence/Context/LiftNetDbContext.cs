using LiftNet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Persistence.Context
{
    public class LiftNetDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public LiftNetDbContext(DbContextOptions<LiftNetDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ContextConfig.Configure(modelBuilder);
        }

        public DbSet<Domain.Entities.LiftNetVersion> Versions { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentParticipant> AppointmentParticipants { get; set; }
        public DbSet<SocialConnection> SocialConnections { get; set; }
        public DbSet<CoachExtension> CoachExtensions { get; set; }

        // vn geo
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }

        // jobs
        public DbSet<SystemJob> SystemJobs { get; set; }
        public DbSet<CustomerJob> CustomerJobs { get; set; }
        public DbSet<ActionJob> ActionJobs { get; set; }

        // ml 
        public DbSet<SocialSimilarityScore> SocialSimilarityScores { get; set; }
    }
}

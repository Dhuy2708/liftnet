 using LiftNet.Domain.Constants;
using LiftNet.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Persistence.Context
{
    public class ContextConfig
    {
        public static void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogins"); });
            modelBuilder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserTokens"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaims"); });

            modelBuilder.Entity<UserRole>()
                        .HasOne<User>()
                        .WithMany(u => u.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();

            modelBuilder.Entity<UserRole>()
                        .HasOne<Role>()
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

            modelBuilder.Entity<User>()
                        .Property(u => u.Avatar)
                        .HasDefaultValue(DomainConstants.DEFAULT_USER_AVATAR);

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Client)
                        .WithMany()
                        .HasForeignKey(a => a.ClientId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Coach)
                        .WithMany()
                        .HasForeignKey(a => a.CoachId)
                        .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

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
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UserRoles"); });
            modelBuilder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UserLogins"); });
            modelBuilder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UserTokens"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RoleClaims"); });
        }
    }
}

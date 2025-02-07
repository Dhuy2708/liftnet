using LiftNet.Domain.Entities;
using LiftNet.Persistence.Context;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.SQL.Context
{
    public class LiftNetDbContext : IdentityDbContext<User>
    {
        public LiftNetDbContext(DbContextOptions<LiftNetDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ContextConfig.Configure(modelBuilder);
        }

    }
}

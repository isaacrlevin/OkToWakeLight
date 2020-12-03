using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OkToWake.Areas.Identity.Data;

namespace OkToWake.Models
{
    public class OkToWakeContext : IdentityDbContext<OkToWakeUser>
    {
        public OkToWakeContext(DbContextOptions<OkToWakeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<TimeInterval> TimeIntervals { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TestTreesAPI.Models;

namespace TestTreesAPI.DataAccess
{
    public class ExceptionLogDbContext : DbContext
    {
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }

        public ExceptionLogDbContext(DbContextOptions<ExceptionLogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.OwnsOne(x => x.Data);
            });
        }
    }
}

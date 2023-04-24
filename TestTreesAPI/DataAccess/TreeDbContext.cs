using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TestTreesAPI.Models;

namespace TestTreesAPI.DataAccess
{
    public class TreeDbContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }

        public TreeDbContext(DbContextOptions<TreeDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Node>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name);
                entity
                .HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentNodeId)
                .OnDelete(DeleteBehavior.NoAction);
            });

        }
    }
}

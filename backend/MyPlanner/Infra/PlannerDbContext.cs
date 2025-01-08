using Microsoft.EntityFrameworkCore;
using MyPlanner.Models;

namespace MyPlanner.Infra;

public class PlannerDbContext : DbContext
{
    public PlannerDbContext(DbContextOptions<PlannerDbContext> options) :base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.MobilePhone)
            .IsUnique();
    }
}
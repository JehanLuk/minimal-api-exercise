using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MinimalAPI.Infrastructure.Db;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Administrator> Administrators { get; set; } = default!;

    public DbSet<Vehicle> Vehicles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator {
                Id = 1,
                Email = "admin@test.com",
                Password = "1234",
                Profile = "Adm"
            }
        );
    }
}
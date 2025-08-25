using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MinimalAPI.Infrastructure.Db;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Administrator> Administrators { get; set; } = default!;
}
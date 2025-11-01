using FiapCloudGamesTechChallenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Context;

public class ContextDb : DbContext
{
    public ContextDb(DbContextOptions<ContextDb> options)
        : base(options)
    {
    }

    public DbSet<Game> Games { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextDb).Assembly);
    }
}
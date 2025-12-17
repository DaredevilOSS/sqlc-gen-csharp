using Microsoft.EntityFrameworkCore;
using System;

namespace SqliteEFCoreImpl;

public class SalesDbContext(string connectionString) : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DateTime to TEXT conversion for SQLite
        modelBuilder.Entity<Order>()
            .Property(o => o.OrderedAt)
            .HasConversion(
                v => v.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF"),
                v => DateTime.Parse(v));

        modelBuilder.Entity<Customer>()
            .Property(c => c.RegisteredAt)
            .HasConversion(
                v => v.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF"),
                v => DateTime.Parse(v));

        modelBuilder.Entity<Product>()
            .Property(p => p.AddedAt)
            .HasConversion(
                v => v.ToString("yyyy-MM-dd HH:mm:ss.FFFFFFF"),
                v => DateTime.Parse(v));
    }
}
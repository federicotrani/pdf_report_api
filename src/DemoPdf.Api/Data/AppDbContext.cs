using DemoPdf.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoPdf.Api.Data;

/// <summary>
/// DbContext de la aplicación (SQL Server o In-Memory según configuración).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(120).IsRequired();
            entity.Property(u => u.FullName).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(40).IsRequired().HasDefaultValue("User");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouses");
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Code).HasMaxLength(20).IsRequired();
            entity.Property(w => w.Name).HasMaxLength(100).IsRequired();
            entity.Property(w => w.Location).HasMaxLength(200);
            entity.Property(w => w.IsActive).HasDefaultValue(true);
            entity.HasIndex(w => w.Code).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Sku).HasMaxLength(40).IsRequired();
            entity.Property(p => p.Name).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.HasIndex(p => p.Sku).IsUnique();

            entity.HasOne(p => p.Warehouse)
                .WithMany(w => w.Products)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

using DemoPdf.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoPdf.Api.Data;

/// <summary>
/// Carga datos de demostración solo si las tablas están vacías.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Warehouses.AnyAsync())
        {
            var central = new Warehouse
            {
                Code = "DEP-001",
                Name = "Depósito Central",
                Location = "Asunción - Planta Baja",
                IsActive = true
            };
            var norte = new Warehouse
            {
                Code = "DEP-002",
                Name = "Depósito Zona Norte",
                Location = "Luque - Km 12",
                IsActive = true
            };
            var sur = new Warehouse
            {
                Code = "DEP-003",
                Name = "Depósito Zona Sur",
                Location = "Ñemby - Ruta 1",
                IsActive = false
            };
            db.Warehouses.AddRange(central, norte, sur);
            await db.SaveChangesAsync();

            var now = DateTime.UtcNow;
            db.Products.AddRange(
                new Product { Sku = "SKU-1001", Name = "Teclado Mecánico", Description = "Teclado mecánico retroiluminado", Price = 450000m, Stock = 25, WarehouseId = central.Id, CreatedAt = now },
                new Product { Sku = "SKU-1002", Name = "Mouse Inalámbrico", Description = "Mouse inalámbrico 2.4GHz", Price = 120000m, Stock = 60, WarehouseId = central.Id, CreatedAt = now },
                new Product { Sku = "SKU-1003", Name = "Monitor 24\"", Description = "Monitor LED Full HD 24 pulgadas", Price = 1350000m, Stock = 12, WarehouseId = central.Id, CreatedAt = now },
                new Product { Sku = "SKU-2001", Name = "Notebook 14\"", Description = "Notebook 14\" 8GB RAM 256GB SSD", Price = 4200000m, Stock = 8, WarehouseId = norte.Id, CreatedAt = now },
                new Product { Sku = "SKU-2002", Name = "Impresora Láser", Description = "Impresora láser monocromática", Price = 980000m, Stock = 5, WarehouseId = norte.Id, CreatedAt = now },
                new Product { Sku = "SKU-2003", Name = "Cable HDMI 2m", Description = "Cable HDMI 2.0 de 2 metros", Price = 35000m, Stock = 150, WarehouseId = norte.Id, CreatedAt = now },
                new Product { Sku = "SKU-3001", Name = "Silla Ergonómica", Description = "Silla de oficina ergonómica", Price = 1750000m, Stock = 10, WarehouseId = sur.Id, CreatedAt = now },
                new Product { Sku = "SKU-3002", Name = "Escritorio Metálico", Description = "Escritorio metálico 1.20m", Price = 890000m, Stock = 7, WarehouseId = sur.Id, CreatedAt = now },
                new Product { Sku = "SKU-3003", Name = "Auriculares Bluetooth", Description = "Auriculares con cancelación de ruido", Price = 320000m, Stock = 0, WarehouseId = sur.Id, CreatedAt = now },
                new Product { Sku = "SKU-1004", Name = "Webcam Full HD", Description = null, Price = 210000m, Stock = 30, WarehouseId = central.Id, CreatedAt = now }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Users.AnyAsync())
        {
            var now = DateTime.UtcNow;
            db.Users.AddRange(
                new User { Username = "admin", Email = "admin@demopdf.com", FullName = "Administrador del Sistema", Role = "Admin", CreatedAt = now, IsActive = true },
                new User { Username = "jperez", Email = "juan.perez@demopdf.com", FullName = "Juan Pérez", Role = "Supervisor", CreatedAt = now, IsActive = true },
                new User { Username = "mgomez", Email = "maria.gomez@demopdf.com", FullName = "María Gómez", Role = "Operador", CreatedAt = now, IsActive = true },
                new User { Username = "cfernandez", Email = "carlos.fernandez@demopdf.com", FullName = "Carlos Fernández", Role = "Operador", CreatedAt = now, IsActive = false },
                new User { Username = "lbareiro", Email = "laura.bareiro@demopdf.com", FullName = "Laura Bareiro", Role = "Auditor", CreatedAt = now, IsActive = true }
            );
            await db.SaveChangesAsync();
        }
    }
}

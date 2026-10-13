using DemoPdf.Api.Application.DTOs;
using DemoPdf.Api.Data;
using DemoPdf.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoPdf.Api.Application.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(int? warehouseId = null, CancellationToken ct = default)
    {
        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(p => p.WarehouseId == warehouseId.Value);

        var products = await query.OrderBy(p => p.Sku).ToListAsync(ct);
        return products.Select(ProductDto.FromEntity).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Warehouse)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return product is null ? null : ProductDto.FromEntity(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        if (!await _db.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId, ct))
            throw new InvalidOperationException($"El depósito con Id {dto.WarehouseId} no existe.");
        if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku, ct))
            throw new InvalidOperationException($"El SKU '{dto.Sku}' ya existe.");

        var product = new Product
        {
            Sku = dto.Sku,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            WarehouseId = dto.WarehouseId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        return (await GetByIdAsync(product.Id, ct))!;
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return null;

        if (!await _db.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId, ct))
            throw new InvalidOperationException($"El depósito con Id {dto.WarehouseId} no existe.");
        if (await _db.Products.AnyAsync(p => p.Sku == dto.Sku && p.Id != id, ct))
            throw new InvalidOperationException($"El SKU '{dto.Sku}' ya existe.");

        product.Sku = dto.Sku;
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.WarehouseId = dto.WarehouseId;

        await _db.SaveChangesAsync(ct);

        return (await GetByIdAsync(product.Id, ct))!;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

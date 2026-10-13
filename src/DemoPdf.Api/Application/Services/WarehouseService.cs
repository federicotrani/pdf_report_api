using DemoPdf.Api.Application.DTOs;
using DemoPdf.Api.Data;
using DemoPdf.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoPdf.Api.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly AppDbContext _db;

    public WarehouseService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<WarehouseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var warehouses = await _db.Warehouses.AsNoTracking().OrderBy(w => w.Code).ToListAsync(ct);
        return warehouses.Select(WarehouseDto.FromEntity).ToList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var warehouse = await _db.Warehouses.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, ct);
        return warehouse is null ? null : WarehouseDto.FromEntity(warehouse);
    }

    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto, CancellationToken ct = default)
    {
        if (await _db.Warehouses.AnyAsync(w => w.Code == dto.Code, ct))
            throw new InvalidOperationException($"El código de depósito '{dto.Code}' ya existe.");

        var warehouse = new Warehouse
        {
            Code = dto.Code,
            Name = dto.Name,
            Location = dto.Location,
            IsActive = dto.IsActive
        };

        _db.Warehouses.Add(warehouse);
        await _db.SaveChangesAsync(ct);
        return WarehouseDto.FromEntity(warehouse);
    }

    public async Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseDto dto, CancellationToken ct = default)
    {
        var warehouse = await _db.Warehouses.FirstOrDefaultAsync(w => w.Id == id, ct);
        if (warehouse is null) return null;

        if (await _db.Warehouses.AnyAsync(w => w.Code == dto.Code && w.Id != id, ct))
            throw new InvalidOperationException($"El código de depósito '{dto.Code}' ya existe.");

        warehouse.Code = dto.Code;
        warehouse.Name = dto.Name;
        warehouse.Location = dto.Location;
        warehouse.IsActive = dto.IsActive;

        await _db.SaveChangesAsync(ct);
        return WarehouseDto.FromEntity(warehouse);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var warehouse = await _db.Warehouses.FirstOrDefaultAsync(w => w.Id == id, ct);
        if (warehouse is null) return false;

        if (await _db.Products.AnyAsync(p => p.WarehouseId == id, ct))
            throw new InvalidOperationException(
                "No se puede eliminar el depósito porque tiene productos asociados. Inactívelo en su lugar.");

        _db.Warehouses.Remove(warehouse);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

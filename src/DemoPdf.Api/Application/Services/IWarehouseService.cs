using DemoPdf.Api.Application.DTOs;

namespace DemoPdf.Api.Application.Services;

public interface IWarehouseService
{
    Task<IReadOnlyList<WarehouseDto>> GetAllAsync(CancellationToken ct = default);
    Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto, CancellationToken ct = default);
    Task<WarehouseDto?> UpdateAsync(int id, UpdateWarehouseDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

namespace DemoPdf.Api.Application.Services;

public interface IReportService
{
    /// <summary>Genera el PDF del listado de productos (filtrable por depósito).</summary>
    Task<byte[]> GetProductsPdfAsync(int? warehouseId = null, CancellationToken ct = default);

    /// <summary>Genera el PDF del listado de usuarios.</summary>
    Task<byte[]> GetUsersPdfAsync(CancellationToken ct = default);
}

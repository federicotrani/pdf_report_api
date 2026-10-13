using DemoPdf.Api.Application.DTOs;
using DemoPdf.Api.Application.Reports;
using DemoPdf.Api.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace DemoPdf.Api.Application.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    private readonly IProductService _productService;

    public ReportService(AppDbContext db, IProductService productService)
    {
        _db = db;
        _productService = productService;
    }

    public async Task<byte[]> GetProductsPdfAsync(int? warehouseId = null, CancellationToken ct = default)
    {
        var products = await _productService.GetAllAsync(warehouseId, ct);

        string? warehouseLabel = null;
        if (warehouseId.HasValue)
        {
            var warehouse = await _db.Warehouses.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == warehouseId.Value, ct);
            warehouseLabel = warehouse is null
                ? null
                : $"{warehouse.Name} ({warehouse.Code})";
        }

        var document = new ProductsReportDocument(products, warehouseLabel);
        return document.GeneratePdf();
    }

    public async Task<byte[]> GetUsersPdfAsync(CancellationToken ct = default)
    {
        var userEntities = await _db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync(ct);
        var users = userEntities.Select(UserDto.FromEntity).ToList();

        var document = new UsersReportDocument(users);
        return document.GeneratePdf();
    }
}

using DemoPdf.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoPdf.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService) => _reportService = reportService;

    /// <summary>
    /// Genera el PDF del listado de productos (filtro opcional por depósito).
    /// </summary>
    [HttpGet("products-pdf")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsPdf([FromQuery] int? warehouseId, CancellationToken ct)
    {
        var pdfBytes = await _reportService.GetProductsPdfAsync(warehouseId, ct);
        if (pdfBytes.Length == 0)
            return NotFound(new { message = "No se pudieron generar el reporte de productos." });

        var fileName = warehouseId.HasValue
            ? $"productos_deposito-{warehouseId}_{DateTime.Now:yyyyMMddHHmmss}.pdf"
            : $"productos_{DateTime.Now:yyyyMMddHHmmss}.pdf";

        return File(new MemoryStream(pdfBytes), "application/pdf", fileName);
    }

    /// <summary>
    /// Genera el PDF del listado de usuarios.
    /// </summary>
    [HttpGet("users-pdf")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersPdf(CancellationToken ct)
    {
        var pdfBytes = await _reportService.GetUsersPdfAsync(ct);
        return File(new MemoryStream(pdfBytes), "application/pdf", $"usuarios_{DateTime.Now:yyyyMMddHHmmss}.pdf");
    }
}

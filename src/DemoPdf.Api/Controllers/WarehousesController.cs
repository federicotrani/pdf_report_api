using DemoPdf.Api.Application.DTOs;
using DemoPdf.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoPdf.Api.Controllers;

[ApiController]
[Route("api/warehouses")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService) => _warehouseService = warehouseService;

    /// <summary>Obtiene todos los depósitos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WarehouseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAll(CancellationToken ct)
        => Ok(await _warehouseService.GetAllAsync(ct));

    /// <summary>Obtiene un depósito por Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WarehouseDto>> GetById(int id, CancellationToken ct)
    {
        var warehouse = await _warehouseService.GetByIdAsync(id, ct);
        return warehouse is null ? NotFound(new { message = $"Depósito {id} no encontrado." }) : Ok(warehouse);
    }

    /// <summary>Crea un nuevo depósito.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _warehouseService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Actualiza un depósito existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WarehouseDto>> Update(int id, [FromBody] UpdateWarehouseDto dto, CancellationToken ct)
    {
        try
        {
            var updated = await _warehouseService.UpdateAsync(id, dto, ct);
            return updated is null
                ? NotFound(new { message = $"Depósito {id} no encontrado." })
                : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>Elimina un depósito (solo si no tiene productos asociados).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            var deleted = await _warehouseService.DeleteAsync(id, ct);
            return deleted ? NoContent() : NotFound(new { message = $"Depósito {id} no encontrado." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}

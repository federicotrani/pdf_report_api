using System.ComponentModel.DataAnnotations;
using DemoPdf.Api.Domain.Entities;

namespace DemoPdf.Api.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseCode { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>Precio × Stock (Total Valorizado).</summary>
    public decimal TotalValue => Price * Stock;

    public static ProductDto FromEntity(Product product) => new()
    {
        Id = product.Id,
        Sku = product.Sku,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock,
        WarehouseId = product.WarehouseId,
        WarehouseName = product.Warehouse?.Name,
        WarehouseCode = product.Warehouse?.Code,
        CreatedAt = product.CreatedAt
    };
}

public class CreateProductDto
{
    [Required, StringLength(40, MinimumLength = 3)]
    public string Sku { get; set; } = default!;

    [Required, StringLength(150)]
    public string Name { get; set; } = default!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un depósito válido.")]
    public int WarehouseId { get; set; }
}

public class UpdateProductDto
{
    [Required, StringLength(40, MinimumLength = 3)]
    public string Sku { get; set; } = default!;

    [Required, StringLength(150)]
    public string Name { get; set; } = default!;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un depósito válido.")]
    public int WarehouseId { get; set; }
}

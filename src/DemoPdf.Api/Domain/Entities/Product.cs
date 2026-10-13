namespace DemoPdf.Api.Domain.Entities;

/// <summary>
/// Producto perteneciente a un depósito.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

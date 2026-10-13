namespace DemoPdf.Api.Domain.Entities;

/// <summary>
/// Depósito / almacén donde se guardan productos.
/// </summary>
public class Warehouse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

using System.ComponentModel.DataAnnotations;
using DemoPdf.Api.Domain.Entities;

namespace DemoPdf.Api.Application.DTOs;

public class WarehouseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public bool IsActive { get; set; }

    public static WarehouseDto FromEntity(Warehouse warehouse) => new()
    {
        Id = warehouse.Id,
        Code = warehouse.Code,
        Name = warehouse.Name,
        Location = warehouse.Location,
        IsActive = warehouse.IsActive
    };
}

public class CreateWarehouseDto
{
    [Required, StringLength(20, MinimumLength = 2)]
    public string Code { get; set; } = default!;

    [Required, StringLength(100)]
    public string Name { get; set; } = default!;

    [StringLength(200)]
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateWarehouseDto
{
    [Required, StringLength(20, MinimumLength = 2)]
    public string Code { get; set; } = default!;

    [Required, StringLength(100)]
    public string Name { get; set; } = default!;

    [StringLength(200)]
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;
}

using System.ComponentModel.DataAnnotations;
using DemoPdf.Api.Domain.Entities;

namespace DemoPdf.Api.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public static UserDto FromEntity(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role,
        CreatedAt = user.CreatedAt,
        IsActive = user.IsActive
    };
}

public class CreateUserDto
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = default!;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = default!;

    [Required, StringLength(150)]
    public string FullName { get; set; } = default!;

    [StringLength(40)]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
}

public class UpdateUserDto
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = default!;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = default!;

    [Required, StringLength(150)]
    public string FullName { get; set; } = default!;

    [StringLength(40)]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
}

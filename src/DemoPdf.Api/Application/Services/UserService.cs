using DemoPdf.Api.Application.DTOs;
using DemoPdf.Api.Data;
using DemoPdf.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoPdf.Api.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await _db.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync(ct);
        return users.Select(UserDto.FromEntity).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        return user is null ? null : UserDto.FromEntity(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username, ct))
            throw new InvalidOperationException($"El nombre de usuario '{dto.Username}' ya está registrado.");
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email, ct))
            throw new InvalidOperationException($"El email '{dto.Email}' ya está registrado.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName,
            Role = dto.Role,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return UserDto.FromEntity(user);
    }

    public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return null;

        if (await _db.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id, ct))
            throw new InvalidOperationException($"El nombre de usuario '{dto.Username}' ya está registrado.");
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id, ct))
            throw new InvalidOperationException($"El email '{dto.Email}' ya está registrado.");

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.FullName = dto.FullName;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;

        await _db.SaveChangesAsync(ct);
        return UserDto.FromEntity(user);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

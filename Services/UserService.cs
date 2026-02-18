using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class UserService(SchoolDbContext context, ILogger<UserService> logger, BaseRepository<User> repo)
    : BaseService<User>(repo)
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<List<User>> GetUsersByTenantAsync(Guid tenantId)
    {
        return await context.Users
            .Where(u => u.TenantId == tenantId)
            .ToListAsync();
    }


    public async Task<User> CreateUserAsync(CreateUserDto dto)
    {
        var tenantExists = await context.Tenants.AnyAsync(t => t.Id == dto.TenantId);
        if (!tenantExists)
            throw new Exception("Tenant not found");

        var emailExists = await context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            throw new Exception("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Role = Enum.Parse<UserRole>(dto.Role),
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        logger.LogInformation("User created with ID {Id}", user.Id);

        return user;
    }
}
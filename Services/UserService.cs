using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
using SchoolSystem.Domain.ValueObjects;

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

    /// <summary>
    /// Update user profile information
    /// </summary>
    public async Task<User?> UpdateUserAsync(Guid userId, UpdateUserDto dto)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return null;

        // Update name if provided
        if (!string.IsNullOrEmpty(dto.FirstName) || !string.IsNullOrEmpty(dto.LastName))
        {
            var firstName = dto.FirstName ?? user.Name?.FirstName ?? "";
            var lastName = dto.LastName ?? user.Name?.LastName ?? "";
            user.Name = new FullName(firstName, "", lastName);
        }

        // Update other fields
        if (!string.IsNullOrEmpty(dto.Phone))
            user.Phone = dto.Phone;

        if (!string.IsNullOrEmpty(dto.Region) || !string.IsNullOrEmpty(dto.City) || 
            !string.IsNullOrEmpty(dto.SubCity) || !string.IsNullOrEmpty(dto.Woreda) || !string.IsNullOrEmpty(dto.HouseNumber))
        {
            var region = dto.Region ?? user.Address?.Region ?? "";
            var city = dto.City ?? user.Address?.City ?? "";
            var subCity = dto.SubCity?? user.Address?.SubCity ?? "";
            var woreda = dto.Woreda ?? user.Address?.Woreda?? "";
            var houseNumber = dto.HouseNumber ?? user.Address?.HouseNumber ?? "";
            user.Address = new Address(region, city, subCity, woreda, houseNumber);
        }

        user.UpdatedAt = DateTime.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        logger.LogInformation("User updated with ID {Id}", user.Id);

        return user;
    }

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        logger.LogInformation("User deleted with ID {Id}", userId);

        return true;
    }

    public async Task<User?> UpdatePasswordAsync (string email, string newPassword )
    {
        var user = await context.Users.Where(user1 => user1.Email == email).FirstAsync();
        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

        await context.SaveChangesAsync();
        return user;
    }
}
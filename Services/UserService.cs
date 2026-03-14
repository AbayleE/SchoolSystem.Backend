using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Users;
using SchoolSystem.Backend.Exceptions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
using SchoolSystem.Domain.ValueObjects;
using SchoolSystem.Backend.Services.AuthService;

namespace SchoolSystem.Backend.Services;

public class UserService(
    TenantRepository<User> repo,
    SchoolDbContext context,
    ITenantContext tenantContext,
    PasswordHasher<User> passwordHasher,
    ILogger<UserService> logger, EmailService emailService)
    : TenantService<User>(repo)
{
    
    public Task<List<User>> GetUsersAsync() => GetAllAsync();

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users
            .Where(u => u.TenantId == tenantContext.TenantId && u.Email == email && !u.IsDeleted)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
    {
        return await context.Users
            .Where(u =>
                u.TenantId == tenantContext.TenantId &&
                u.Role == role &&
                !u.IsDeleted)
            .ToListAsync();
    }
    
    // Update user profile information
    public async Task<User> UpdateUserAsync(Guid userId, UpdateUserDto dto)
    {
        var user = await GetByIdAsync(userId)
                   ?? throw new NotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.FirstName) || !string.IsNullOrWhiteSpace(dto.LastName))
        {
            user.Name = new FullName(
                dto.FirstName ?? user.Name?.FirstName ?? "",
                dto.MiddleName ?? user.Name?.MiddleName ?? "",
                dto.LastName ?? user.Name?.LastName ?? ""
            );
        }

        if (!string.IsNullOrWhiteSpace(dto.Phone))
            user.Phone = dto.Phone;

        // Only update address if any address field is provided
        if (dto.Region != null || dto.City != null || dto.SubCity != null ||
            dto.Woreda != null || dto.HouseNumber != null)
        {
            user.Address = new Address(
                dto.Region ?? user.Address?.Region ?? "",
                dto.City ?? user.Address?.City ?? "",
                dto.SubCity ?? user.Address?.SubCity ?? "",
                dto.Woreda ?? user.Address?.Woreda ?? "",
                dto.HouseNumber ?? user.Address?.HouseNumber ?? ""
            );
        }

        await UpdateAsync(user);
        logger.LogInformation("User {UserId} profile updated", userId);
        return user;
    }
    
    // Delete user (soft delete)
    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var deleted = await DeleteAsync(userId);
        if (deleted) logger.LogInformation("User {UserId} soft deleted", userId);
        return deleted;
    }
    
    public async Task SetActiveStatusAsync(Guid userId, bool isActive)
    {
        var user = await GetByIdAsync(userId)
                   ?? throw new NotFoundException("User not found.");

        user.IsActive = isActive;
        await UpdateAsync(user);

        logger.LogInformation("User {UserId} active status set to {IsActive}", userId, isActive);
    }
    
    public async Task <User> CreateAdminUserAsync(CreateAdminUserDto dto)
    {
        var tenantId = await context.Tenants.Where(tenant => tenant.Name == dto.Tenant).Select(tenant => tenant.Id).FirstAsync();
        if(tenantId == Guid.Empty)
                throw new NotFoundException("Tenant not found.");
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = dto.Email,
            Phone = dto.Phone,
            Role = UserRole.SystemOwner,
            Name = new FullName(dto.FirstName, dto.MiddleName!, dto.LastName),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
      
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        logger.LogInformation("Admin user {UserId} created for tenant {TenantId}", user.Id, tenantId);
        
        return user;
    }

    public async Task<List<User>> CreateUsersBulk(List<CreateUserDto> dtos)
    {
        var users = new List<User>();

        foreach (var dto in dtos)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = new Guid(dto.TenantId),
                Email = dto.Email,
                Phone = dto.Phone,
                Role = (UserRole)int.Parse(dto.Role),
                Name = new FullName(dto.FirstName, "", dto.LastName)
            };
          CreateRoleProfile(user);
            users.Add(user);
        }

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        return users;
    }
    
    public void CreateRoleProfile(User user)
    {
        switch (user.Role)
        {
            case UserRole.Teacher:
                context.Teachers.Add(new Teacher
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    Status = TeacherStatus.Active
                });
                break;

            case UserRole.Parent:
                context.Parents.Add(new Parent
                {
                    TenantId = user.TenantId,
                    UserId = user.Id
                });
                break;

            case UserRole.Student:
                var existingProfile = context.Students
                    .FirstOrDefault(s =>
                        s.TenantId == user.TenantId &&
                        s.UserId == Guid.Empty);

                if (existingProfile != null)
                    existingProfile.UserId = user.Id;
                else
                    context.Students.Add(new Student
                    {
                        TenantId = user.TenantId,
                        UserId = user.Id,
                        Status = StudentStatus.Active
                    });
                break;
        }
    }

}

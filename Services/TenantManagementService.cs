using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Tenants;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class TenantManagementService(SchoolDbContext context, ILogger<TenantManagementService> logger)

{
    public async Task<Tenant> CreateTenantAsync(CreateTenantDto dto)
    {
        var subdomainTaken = await context.Tenants
            .AnyAsync(t => t.Subdomain == dto.Subdomain && !t.IsDeleted);
        if (subdomainTaken)
            throw new InvalidOperationException($"Subdomain '{dto.Subdomain}' is already taken.");

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Subdomain = dto.Subdomain,
            LogoUrl = dto.LogoUrl,
            Status = TenantStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        logger.LogInformation("Tenant {name} created with ID: {Id} ",tenant.Name, tenant.Id);
        return tenant;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id)
    {
        return await context.Tenants.FindAsync(id);
    }
    
    public async Task SetTenantStatusAsync(Guid tenantId, TenantStatus status)
    {
        var tenant = await context.Tenants.FindAsync(tenantId)
                     ?? throw new NotFoundException("Tenant not found.");

        tenant.Status = status;
        tenant.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        logger.LogInformation("Tenant {TenantId} status set to {Status}", tenantId, status);
    }
}
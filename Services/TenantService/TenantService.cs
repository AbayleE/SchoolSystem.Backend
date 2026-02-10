using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Tenants;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.TenantService;

public class TenantService(SchoolDbContext context, ILogger<TenantService> logger) : ITenantService
{
    public async Task<List<Tenant>> GetAllTenantsAsync()
    {
        logger.LogInformation("Getting a list of Tenants");

        return await context.Tenants.ToListAsync();
    }

    public async Task<Tenant> GetTenantByIdAsync(Guid id)
    {
        logger.LogInformation("Getting Tenant with ID: {Id}", id);

        return await context.Tenants.FindAsync(id) ?? throw new InvalidOperationException("Tenant not found");
    }

    public async Task<Tenant> CreateTenantAsync(CreateTenantDto dto)
    {
        logger.LogInformation("Creating tenant: {Name}", dto.Name);

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            LogoUrl = dto.LogoUrl,
            Subdomain = dto.Subdomain,
            CreatedAt = DateTime.UtcNow
        };

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        logger.LogInformation("Tenant created with ID: {Id}", tenant.Id);

        return tenant;
    }
}
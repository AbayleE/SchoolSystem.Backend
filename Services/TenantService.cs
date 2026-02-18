using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Tenants;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class TenantService(SchoolDbContext context, ILogger<TenantService> logger)

{
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

    public async Task<Tenant?> GetByIdAsync(Guid id)
    {
        return await context.Tenants.FindAsync(id);
    }
}
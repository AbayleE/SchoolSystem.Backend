using SchoolSystem.Backend.DTOs.Tenants;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.TenantService;

public interface ITenantService
{
    Task<List<Tenant>> GetAllTenantsAsync();
    Task<Tenant> GetTenantByIdAsync(Guid id);
    Task<Tenant> CreateTenantAsync(CreateTenantDto dto);
}
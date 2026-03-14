using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SchoolSystem.Backend.DTOs.Notifications;
using SchoolSystem.Backend.DTOs.Tenants;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Entities;
using SetTenantStatusDto = SchoolSystem.Backend.DTOs.Tenants.SetTenantStatusDto;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController(TenantManagementService tenantManagementService) : ControllerBase
{
    [HttpGet("query")]
    [AllowAnonymous]
    [EnableQuery(PageSize = 100, MaxExpansionDepth = 3)]
    public IQueryable<Tenant> Query() => tenantManagementService.GetQueryable();
    
    [Authorize(Roles = "SystemOwner")]
    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
    {
        var tenant = await tenantManagementService.CreateTenantAsync(dto);
        return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
    }

    //[Authorize(Roles = "SystemOwner")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTenant(Guid id)
    {
        var tenant = await tenantManagementService.GetByIdAsync(id);
        return tenant == null ? NotFound() : Ok(tenant);
    }

    [Authorize(Roles = "SystemOwner")]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetTenantStatusDto dto)
    {
        await tenantManagementService.SetTenantStatusAsync(id, dto.Status);
        return Ok(new { message = $"Tenant status updated to {dto.Status}" });
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTenants()
        => Ok(await tenantManagementService.GetTenantsAsync());
}

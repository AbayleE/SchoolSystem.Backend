using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Tenants;
using SchoolSystem.Backend.Services;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController(TenantService tenantService) : ControllerBase
{
    [Authorize(Roles = "SystemOwner")]
    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
    {
        var tenant = await tenantService.CreateTenantAsync(dto);
        return Ok(tenant);
    }

    [Authorize(Roles = "SystemOwner")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var tenant = await tenantService.GetByIdAsync(id);
        return Ok(tenant);
    }
}
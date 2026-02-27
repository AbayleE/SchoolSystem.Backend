using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SystemOwner")]
public class SystemSettingsController(SystemSettingsService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await service.GetAsync());
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SystemSettings settings)
    {
        return Ok(await service.UpdateAsync(settings));
    }
}
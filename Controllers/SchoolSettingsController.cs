using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Exceptions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SchoolSettingsController(SchoolDbContext context, ITenantContext tenant) : ControllerBase
{
    // GET /api/schoolsettings
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await context.SchoolSettings
                           .FirstOrDefaultAsync(s => s.TenantId == tenant.TenantId && !s.IsDeleted)
                       ?? throw new NotFoundException("School settings not found.");
        return Ok(settings);
    }

    // PUT /api/schoolsettings
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SchoolSettings dto)
    {
        var settings = await context.SchoolSettings
                           .FirstOrDefaultAsync(s => s.TenantId == tenant.TenantId && !s.IsDeleted)
                       ?? throw new NotFoundException("School settings not found.");

        settings.GradingSystem = dto.GradingSystem;
        settings.AcademicCalendarType = dto.AcademicCalendarType;
        settings.TimeZone = dto.TimeZone;
        settings.DefaultLanguage = dto.DefaultLanguage;
        settings.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok(settings);
    }
}
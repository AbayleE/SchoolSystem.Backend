using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Applications;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ApplicationsController(
    ApplicationService applicationService) : ControllerBase
{
    // POST /api/applications/{academicYearId}
    // Public — no auth. TenantId resolved from subdomain middleware.
    [HttpPost("{academicYearId:guid}")]
    public async Task<IActionResult> CreateApplication(
        Guid academicYearId,
        [FromBody] CreateApplicationDto dto)
    {
        var application = await applicationService.CreateApplicationAsync(dto, academicYearId);
        return CreatedAtAction(
            nameof(GetApplicationById),
            new { id = application.Id },
            new { id = application.Id, message = "Application submitted successfully" });
    }

    // GET /api/applications
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet]
    public async Task<IActionResult> GetApplications([FromQuery] ApplicationFilterDto filter)
    {
        var (items, total) = await applicationService.GetApplicationsAsync(filter);
        return Ok(new
        {
            data = items,
            pagination = new
            {
                filter.PageNumber,
                filter.PageSize,
                totalRecords = total,
                totalPages = (int)Math.Ceiling(total / (double)filter.PageSize)
            }
        });
    }

    // GET /api/applications/{id}
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetApplicationById(Guid id)
    {
        var application = await applicationService.GetApplicationByIdAsync(id);
        return application == null ? NotFound() : Ok(application);
    }

    // PUT /api/applications/{id}/review
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpPut("{id:guid}/review")]
    public async Task<IActionResult> ReviewApplication(Guid id, [FromBody] ReviewApplicationDto dto)
    {
        var reviewedBy = User.GetUserId();
        var application = await applicationService.ReviewApplicationAsync(id, dto, reviewedBy);
        return Ok(new { message = "Application reviewed successfully", data = application });
    }

    // POST /api/applications/{id}/documents
    [HttpPost("{id:guid}/documents")]
    public async Task<IActionResult> UploadDocument(
        Guid id,
        IFormFile file,
        [FromQuery] FileType fileType,
        [FromQuery] string? description)
    {
        var resource = await applicationService.UploadDocumentAsync(id, file, fileType, description);
        return Ok(resource);
    }
}


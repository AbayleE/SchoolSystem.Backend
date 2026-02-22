using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.DTOs.Applications;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsEnhancedController(
    ApplicationServiceEnhanced applicationService,
    ITenantContext tenantContext,
    ILogger<ApplicationsEnhancedController> logger) : ControllerBase
{
    // ---------------------------------------------------------
    // POST /api/applications
    // Accept all form fields, validate, store to database
    // No auth required (public endpoint)
    // ---------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto dto)
    {
        try
        {
            // Get tenant from header or context
            var tenantId = HttpContext.GetTenantIdFromHeader();
            if (tenantId == Guid.Empty && tenantContext.TenantId != Guid.Empty)
                tenantId = tenantContext.TenantId;
            var application = await applicationService.CreateApplicationAsync(dto, tenantId);

            return CreatedAtAction(
                nameof(GetApplicationById),
                new { id = application.Id },
                new { id = application.Id, message = "Application submitted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid application data");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating application");
            return StatusCode(500, new { message = "An error occurred while processing your application" });
        }
    }

    // ---------------------------------------------------------
    // GET /api/applications
    // List all applications with pagination and filtering
    // Admin/SchoolAdmin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet]
    public async Task<IActionResult> GetApplications([FromQuery] ApplicationFilterDto filter)
    {
        try
        {
            var (applications, total) = await applicationService.GetApplicationsAsync(filter, tenantContext.TenantId);

            return Ok(new
            {
                data = applications,
                pagination = new
                {
                    pageNumber = filter.PageNumber,
                    pageSize = filter.PageSize,
                    totalRecords = total,
                    totalPages = (int)System.Math.Ceiling(total / (double)filter.PageSize)
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching applications");
            return StatusCode(500, new { message = "An error occurred while fetching applications" });
        }
    }

    // ---------------------------------------------------------
    // GET /api/applications/:id
    // Return application details
    // Admin/SchoolAdmin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetApplicationById(Guid id)
    {
        try
        {
            var application = await applicationService.GetApplicationByIdAsync(id, tenantContext.TenantId);

            if (application == null)
                return NotFound(new { message = "Application not found" });

            return Ok(application);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching application");
            return StatusCode(500, new { message = "An error occurred while fetching the application" });
        }
    }

    // ---------------------------------------------------------
    // PUT /api/applications/:id
    // Update status (pending/accepted/rejected), add notes
    // Send notification email
    // Admin only
    // ---------------------------------------------------------
    [Authorize(Roles = "SystemOwner, SchoolAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateApplicationStatus(
        Guid id,
        [FromBody] UpdateApplicationStatusDto dto)
    {
        try
        {
            var application = await applicationService.UpdateApplicationStatusAsync(
                id,
                dto.Status,
                dto.Notes,
                tenantContext.TenantId,
                dto.SendNotification);

            if (application == null)
                return NotFound(new { message = "Application not found" });

            return Ok(new { message = "Application status updated successfully", data = application });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating application status");
            return StatusCode(500, new { message = "An error occurred while updating the application" });
        }
    }
}

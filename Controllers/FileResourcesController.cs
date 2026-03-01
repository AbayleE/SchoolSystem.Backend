using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileResourcesController(FileResourceService fileService, ITenantContext tenant)
    : ControllerBase
{
    // POST /api/fileresources/upload
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromQuery] FileType fileType,
        [FromQuery] Guid? relatedEntityId,
        [FromQuery] string? relatedEntityType)
    {
        var resource = await fileService.UploadAsync(file, fileType, relatedEntityId, relatedEntityType);
        return Ok(resource);
    }

    // GET /api/fileresources/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var resource = await fileService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        if (resource.TenantId.HasValue && resource.TenantId != tenant.TenantId)
            return Forbid();

        return Ok(resource);
    }

    // GET /api/fileresources/entity/{entityId}
    [HttpGet("entity/{entityId:guid}")]
    public async Task<IActionResult> GetByEntity(Guid entityId)
    {
        var files = await fileService.GetByEntityAsync(entityId, tenant.TenantId);
        return Ok(files);
    }

    // DELETE /api/fileresources/{id}
    [Authorize(Roles = "SystemOwner,SchoolAdmin,Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var resource = await fileService.GetByIdAsync(id);
        if (resource == null) return NotFound();

        if (resource.TenantId.HasValue && resource.TenantId != tenant.TenantId)
            return Forbid();

        await fileService.DeleteAsync(id);
        return NoContent();
    }
}
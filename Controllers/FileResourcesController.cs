using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileResourcesController(FileResourceService fileService, SchoolDbContext context, ITenantContext tenant)
    : ControllerBase
{
    // POST /api/fileresources/upload
    [Authorize]
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
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var resource = await context.FileResources
                           .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted)
                       ?? throw new NotFoundException("File not found.");
        return Ok(resource);
    }

    // GET /api/fileresources/entity/{entityId} — all files for a related entity
    [Authorize]
    [HttpGet("entity/{entityId:guid}")]
    public async Task<IActionResult> GetByEntity(Guid entityId)
    {
        var files = await context.FileResources
            .Where(f => f.RelatedEntityId == entityId && !f.IsDeleted)
            .ToListAsync();
        return Ok(files);
    }

    // DELETE /api/fileresources/{id}
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var resource = await context.FileResources
                           .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted)
                       ?? throw new NotFoundException("File not found.");

        resource.IsDeleted = true;
        resource.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return NoContent();
    }
}
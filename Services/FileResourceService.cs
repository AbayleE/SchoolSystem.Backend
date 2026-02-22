using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class FileResourceService(SchoolDbContext context, ITenantContext tenant)
{
    private readonly SchoolDbContext _context = context;
    private readonly ITenantContext _tenant = tenant;

    public async Task<FileResource> UploadAsync(IFormFile file, FileType fileType = FileType.AssignmentSubmission)
    {
        var id = Guid.NewGuid();
        var fileName = file.FileName;
        var url = $"/uploads/{id}/{fileName}";

        var resource = new FileResource
        {
            Id = id,
            TenantId = _tenant.TenantId,
            Url = url,
            FileName = fileName,
            FileSize = file.Length,
            FileType = fileType,
            RelatedEntityId = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.FileResources.Add(resource);
        await _context.SaveChangesAsync();
        return resource;
    }
}
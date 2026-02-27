using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class FileResourceService(SchoolDbContext context, ITenantContext tenant,  ILogger<FileResourceService> logger)
{

    public async Task<FileResource> UploadAsync(IFormFile file,  FileType fileType,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null)
    {
        var id = Guid.NewGuid();
        var url = $"/uploads/{id}/{file.FileName}";

        var resource = new FileResource
        {
            Id = id,
            TenantId = tenant.TenantId,
            FileUrl = url,
            FileName = file.FileName,
            FileSize = file.Length,
            FileType = fileType,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
        };

        resource.Id = id;
        resource.CreatedAt = DateTime.UtcNow;
        resource.UpdatedAt = DateTime.UtcNow;
        
        context.FileResources.Add(resource);
        await context.SaveChangesAsync();
        
        logger.LogInformation("File {FileName} uploaded as {FileId}", file.FileName, id);
        return resource;
    }
    
    public Task<FileResource> UploadApplicationDocumentAsync(
        IFormFile file,
        FileType fileType,
        Guid applicationId)
    {
        return UploadAsync(file, fileType, applicationId, nameof(Application));
    }
}
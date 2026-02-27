using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Applications;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;
using SchoolSystem.Domain.ValueObjects;

namespace SchoolSystem.Backend.Services;
public class NotFoundException(string message) : Exception(message);

public class ApplicationService(
    SchoolDbContext context,
    BaseRepository<Application> repo,
    ITenantContext tenantContext,
    EmailService emailService,
    ILogger<ApplicationService> logger) : BaseService<Application>(repo)
{
   
    // Create new application (public endpoint - no auth required)

    public async Task<Application> CreateApplicationAsync(
        CreateApplicationDto dto,
        Guid academicYearId)
    {
        if (tenantContext.TenantId == Guid.Empty)
            throw new InvalidOperationException("School not found.");

        // Validate academic year belongs to this school
        // SchoolId in domain == TenantId in the AcademicYear entity
        var academicYear = await context.AcademicYears
                               .FirstOrDefaultAsync(a =>
                                   a.Id == academicYearId &&
                                   a.TenantId == tenantContext.TenantId &&
                                   !a.IsDeleted)
                           ?? throw new NotFoundException("Academic year not found for this school.");

        // Validate grade level belongs to this school
        var gradeLevel = await context.GradeLevels
                             .FirstOrDefaultAsync(g =>
                                 g.Id == dto.GradeAppliedId &&
                                 g.TenantId == tenantContext.TenantId &&
                                 !g.IsDeleted)
                         ?? throw new NotFoundException("Grade level not found for this school.");


        if (!dto.Guardians.Any(g => g.IsPrimaryContact))
            throw new InvalidOperationException("At least one guardian must be marked as the primary contact.");

        var application = new Application
        {
            SchoolId = academicYear.TenantId,
            AcademicYearId = academicYear.Id,
            StudentName = new FullName(dto.FirstName, dto.MiddleName ?? "", dto.LastName),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            GradeAppliedId = dto.GradeAppliedId,
            CurrentGradeLevel = dto.CurrentGradeLevel,
            Gpa = dto.Gpa,
            Address = new Address(
                dto.Region,
                dto.City,
                dto.SubCity ?? "",
                dto.Woreda ?? "",
                dto.HouseNumber ?? ""),
            Guardians = dto.Guardians.Select(g => new Guardian(
              g.FirstName,
              g.LastName,
              g.Email,
              g.Phone,
              g.Relationship,
              g.IsPrimaryContact,
              g.MiddleName)).ToList(),
            Status = ApplicationStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        await AddAsync(application);

        logger.LogInformation(
            "Application {ApplicationId} submitted to school {SchoolId} for academic year {AcademicYearId}",
            application.Id, application.SchoolId, academicYearId);

        // Send confirmation email (optional)
        // await _emailService.SendApplicationConfirmationEmailAsync(dto.Email, application.Id);

        return application;
    }


    // List all applications with pagination and filtering (admin only)
   
    public async Task<(List<Application> Items, int Total)> GetApplicationsAsync(ApplicationFilterDto filter)
    {

        if (tenantContext.TenantId == Guid.Empty)
            throw new InvalidOperationException("School not found.");

        var query = context.Applications
            .Where(a => a.SchoolId == tenantContext.TenantId && !a.IsDeleted);

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);

        if (filter.AcademicYearId.HasValue)
            query = query.Where(a => a.AcademicYearId == filter.AcademicYearId.Value);

        var total = await query.CountAsync();
       
        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "submittedat" => filter.Descending
                ? query.OrderByDescending(a => a.SubmittedAt)
                : query.OrderBy(a => a.SubmittedAt),
            "status" => filter.Descending ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            _ => query.OrderByDescending(a => a.SubmittedAt)
        };

        var applications = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (applications, total);
    }

 
    // Get single application details (admin only)
    
    public async Task<Application?> GetApplicationByIdAsync(Guid applicationId)
    {
        if (tenantContext.TenantId == Guid.Empty)
            throw new InvalidOperationException("School not found.");

        return await context.Applications
            .Include(a => a.AcademicYear)
            .Include(a => a.GradeApplied)
            .FirstOrDefaultAsync(a =>
                a.Id == applicationId &&
                a.SchoolId == tenantContext.TenantId &&
                !a.IsDeleted);
    }


    // Update application status and send notification

    public async Task<Application> ReviewApplicationAsync(
        Guid applicationId,
        ReviewApplicationDto dto,
        Guid reviewedByUserId)
    {
        var application = await GetApplicationByIdAsync(applicationId)
                          ?? throw new NotFoundException("Application not found.");

        if (application.Status != ApplicationStatus.Pending &&
            application.Status != ApplicationStatus.UnderReview)
            throw new InvalidOperationException(
                "Only pending or under-review applications can be reviewed.");

        application.Status = dto.Status;
        application.ReviewNotes = dto.Notes;
        application.ReviewedAt = DateTime.UtcNow;
        application.ReviewedByUserId = reviewedByUserId;

        await UpdateAsync(application);
        
        var primaryGuardian = application.Guardians.FirstOrDefault(g => g.IsPrimaryContact)
                              ?? application.Guardians.FirstOrDefault();
        if (primaryGuardian != null)
        {
            var subject = dto.Status switch
            {
                ApplicationStatus.Accepted    => "Application Accepted",
                ApplicationStatus.Rejected    => "Application Update",
                ApplicationStatus.UnderReview => "Application Under Review",
                _                             => "Application Status Update"
            };

            var message = dto.Status switch
            {
                ApplicationStatus.Accepted    => "Congratulations! Your application has been accepted.",
                ApplicationStatus.Rejected    => "Unfortunately, your application was not accepted at this time.",
                ApplicationStatus.UnderReview => "Your application is now under review. We will be in touch shortly.",
                _                             => "Your application status has been updated."
            };

            //await emailService.SendEmailAsync(primaryGuardian.Email, subject, message);
        }

        logger.LogInformation(
            "Application {ApplicationId} reviewed by {UserId}: {Status}",
            applicationId, reviewedByUserId, dto.Status);

        return application;
    }
    
    public async Task<FileResource> UploadDocumentAsync(
        Guid applicationId,
        IFormFile file,
        FileType fileType,
        string? description)
    {
        // Ownership check — application must belong to this school
        var application = await GetApplicationByIdAsync(applicationId)
                          ?? throw new NotFoundException("Application not found.");

        if (application.Status == ApplicationStatus.Accepted ||
            application.Status == ApplicationStatus.Rejected)
            throw new InvalidOperationException(
                "Cannot upload documents for a finalised application.");

        var id = Guid.NewGuid();
        // In production: upload to S3/Azure Blob, store the returned URL
        var url = $"/uploads/applications/{applicationId}/{id}/{file.FileName}";

        var resource = new FileResource
        {
            Id = id,
            TenantId = null,                          // no tenant yet — pre-enrollment
            FileUrl = url,
            FileName = file.FileName,
            FileSize = file.Length,
            FileType = fileType,
            RelatedEntityId = applicationId,
            RelatedEntityType = nameof(Application),
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.FileResources.Add(resource);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Document {FileId} uploaded for application {ApplicationId}",
            id, applicationId);

        return resource;
    }

}


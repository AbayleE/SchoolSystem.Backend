using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Applications;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class ApplicationServiceEnhanced(
    SchoolDbContext context,
    BaseRepository<Application> repo,
    ITenantContext tenant,
    EmailService emailService,
    ILogger<ApplicationServiceEnhanced> logger) : BaseService<Application>(repo)
{
    /// <summary>
    /// Create new application (public endpoint - no auth required)
    /// </summary>
    public async Task<Application> CreateApplicationAsync(CreateApplicationDto dto, Guid tenantId)
    {
        logger.LogInformation("Creating application for tenant {TenantId}", tenantId);

        // Validate email format
        if (!IsValidEmail(dto.Email) || !IsValidEmail(dto.ParentEmail))
            throw new InvalidOperationException("Invalid email format");

        // Validate phone format
        if (!IsValidPhone(dto.Phone) || !IsValidPhone(dto.ParentPhone))
            throw new InvalidOperationException("Invalid phone format");

        var application = new Application
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AcademicYearId = dto.AcademicYearId,
            StudentName = new Domain.ValueObjects.FullName(dto.FirstName, "", dto.LastName),
            ParentName = new Domain.ValueObjects.FullName("", "", ""),
            ParentEmail = dto.ParentEmail,
            ParentPhone = dto.ParentPhone,
            Address = new Domain.ValueObjects.Address(dto.Region, dto.City ?? "", dto.SubCity ?? "", dto.Woreda ?? "", ""),
            Status = ApplicationStatus.Pending,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Applications.Add(application);
        await context.SaveChangesAsync();

        logger.LogInformation("Application created with ID {ApplicationId}", application.Id);

        // Send confirmation email (optional)
        // await _emailService.SendApplicationConfirmationEmailAsync(dto.Email, application.Id);

        return application;
    }

    /// <summary>
    /// List all applications with pagination and filtering (admin only)
    /// </summary>
    public async Task<(List<Application> Items, int Total)> GetApplicationsAsync(ApplicationFilterDto filter, Guid tenantId)
    {
        logger.LogInformation("Fetching applications for tenant {TenantId}", tenantId);

        var query = context.Applications
            .Where(a => a.TenantId == tenantId);

        // Apply status filter
        if (!string.IsNullOrEmpty(filter.Status))
        {
            if (Enum.TryParse<ApplicationStatus>(filter.Status, true, out var status))
                query = query.Where(a => a.Status == status);
        }

        // Get total count before pagination
        var total = await query.CountAsync();

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "submittedat" => filter.Descending ? query.OrderByDescending(a => a.SubmittedAt) : query.OrderBy(a => a.SubmittedAt),
            "status" => filter.Descending ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            _ => query.OrderByDescending(a => a.SubmittedAt) // default
        };

        // Apply pagination
        var applications = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (applications, total);
    }

    /// <summary>
    /// Get single application details (admin only)
    /// </summary>
    public async Task<Application?> GetApplicationByIdAsync(Guid id, Guid tenantId)
    {
        logger.LogInformation("Fetching application {ApplicationId}", id);

        return await context.Applications
            .Where(a => a.Id == id && a.TenantId == tenantId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Update application status and send notification
    /// </summary>
    public async Task<Application?> UpdateApplicationStatusAsync(
        Guid id,
        string newStatus,
        string? notes,
        Guid tenantId,
        bool sendNotification = true)
    {
        logger.LogInformation("Updating application {ApplicationId} status to {Status}", id, newStatus);

        var application = await GetApplicationByIdAsync(id, tenantId);
        if (application == null)
            return null;

        if (Enum.TryParse<ApplicationStatus>(newStatus, true, out var status))
            application.Status = status;

        application.ReviewedAt = DateTime.UtcNow;
        application.UpdatedAt = DateTime.UtcNow;

        context.Applications.Update(application);
        await context.SaveChangesAsync();

        // Send notification email (optional)
        if (sendNotification && !string.IsNullOrEmpty(application.ParentEmail))
        {
            var message = status switch
            {
                ApplicationStatus.Accepted => "Congratulations! Your application has been accepted.",
                ApplicationStatus.Rejected => "Unfortunately, your application was not accepted.",
                _ => "Your application status has been updated."
            };
            // await _emailService.SendApplicationStatusEmailAsync(application.ParentEmail, message);
        }

        return application;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPhone(string phone)
    {
        // Simple phone validation - at least 10 digits
        var digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
        return digitsOnly.Length >= 10;
    }
}

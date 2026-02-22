using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Contact;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class ContactMessageService(
    SchoolDbContext context,
    ITenantContext tenant,
    EmailService emailService,
    ILogger<ContactMessageService> logger)
{
    private readonly SchoolDbContext _context = context;
    private readonly ITenantContext _tenant = tenant;
    private readonly EmailService _emailService = emailService;
    private readonly ILogger<ContactMessageService> _logger = logger;

    /// <summary>
    /// Create contact message (public endpoint - no auth required)
    /// </summary>
    public async Task<ContactMessage> CreateContactMessageAsync(CreateContactMessageDto dto, Guid tenantId)
    {
        _logger.LogInformation("Creating contact message from {Email}", dto.Email);

        // Validate email format
        if (!IsValidEmail(dto.Email))
            throw new InvalidOperationException("Invalid email format");

        var contactMessage = new ContactMessage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Subject = dto.Subject,
            Message = dto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Add(contactMessage);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Contact message created with ID {ContactMessageId}", contactMessage.Id);

        // Send to admin email (optional)
        // var adminEmail = await GetSchoolAdminEmailAsync(tenantId);
        // if (adminEmail != null)
        //     await _emailService.SendEmailAsync(adminEmail, $"Contact: {dto.Subject}", dto.Message);

        return contactMessage;
    }

    /// <summary>
    /// Get contact message by ID
    /// </summary>
    public async Task<ContactMessage?> GetContactMessageByIdAsync(Guid messageId, Guid tenantId)
    {
        _logger.LogInformation("Fetching contact message {MessageId}", messageId);

        return await _context.Set<ContactMessage>()
            .Where(c => c.Id == messageId && c.TenantId == tenantId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// List all contact messages (admin only)
    /// </summary>
    public async Task<List<ContactMessage>> GetContactMessagesAsync(Guid tenantId, int pageNumber = 1, int pageSize = 10)
    {
        _logger.LogInformation("Fetching contact messages for tenant {TenantId}", tenantId);

        return await _context.Set<ContactMessage>()
            .Where(c => c.TenantId == tenantId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Mark message as read
    /// </summary>
    public async Task<ContactMessage?> MarkAsReadAsync(Guid messageId, Guid tenantId)
    {
        _logger.LogInformation("Marking contact message {MessageId} as read", messageId);

        var message = await _context.Set<ContactMessage>()
            .Where(c => c.Id == messageId && c.TenantId == tenantId)
            .FirstOrDefaultAsync();

        if (message == null)
            return null;

        message.IsRead = true;
        message.UpdatedAt = DateTime.UtcNow;

        _context.Update(message);
        await _context.SaveChangesAsync();

        return message;
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
}

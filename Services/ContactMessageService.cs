using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Contact;
using SchoolSystem.Backend.Exceptions;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class ContactMessageService(
    BaseRepository<ContactMessage> repo,
    SchoolDbContext context,
    ILogger<ContactMessageService> logger)
    : BaseService<ContactMessage>(repo)
{
    // ContactMessage has no TenantId — it goes to the platform team, not a school
    public async Task<ContactMessage> CreateAsync(CreateContactMessageDto dto)
    {
        var message = new ContactMessage
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Subject = dto.Subject,
            Message = dto.Message,
            IsResolved = false
        };

        await AddAsync(message);
        logger.LogInformation("Contact message received from {Email}", dto.Email);
        return message;
    }

    public async Task<ContactMessage> ResolveAsync(Guid id)
    {
        var message = await GetByIdAsync(id)
                      ?? throw new NotFoundException("Contact message not found.");

        message.IsResolved = true;
        return await UpdateAsync(message);
    }

    public async Task<(List<ContactMessage> Items, int Total)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 10,
        bool unresolvedOnly = false)
    {
        var query = context.ContactMessages
            .Where(c => !c.IsDeleted);

        if (unresolvedOnly)
            query = query.Where(c => !c.IsResolved);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}

using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Invitations;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services.InvitationService;

public class InvitationService(
    SchoolDbContext context,
    IEmailService emailService,
    INotificationService notificationService,
    Logger<IInvitationService> logger) : IInvitationService
{
    public async Task<Invitation> CreateInvitationAsync(CreateInvitationDto dto)
    {
        var token = Guid.NewGuid().ToString("N");

        var sender = await context.Users.FirstOrDefaultAsync(u => u.Id == dto.SenderId);
        if (sender?.Email == null)
            throw new Exception("Sender email not found");

        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            Email = dto.Email,
            Role = Enum.Parse<UserRole>(dto.Role),
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Used = false,
            SentByUserId = dto.SenderId,
            SentByRole = Enum.Parse<UserRole>(dto.SenderRole),
            SendByEmail = dto.SenderEmail
        };

        context.Invitations.Add(invitation);
        await context.SaveChangesAsync();

        await emailService.SendInvitationEmailAsync(sender.Email, invitation);
        // await notificationService.CreateInvitationNotificationAsync(invitation);

        return invitation;
    }


    public async Task<Invitation?> GetInvitationByTokenAsync(string token)
    {
        logger.LogInformation("Getting invitation by token {token}", token);
        return await context.Invitations
            .FirstOrDefaultAsync(i => i.Token == token && !i.Used && i.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<Invitation?> GetInvitationByIdAsync(Guid id)
    {
        logger.LogInformation("Getting invitation by id {id}", id);
        return await context.Invitations
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Invitation>> GetInvitationsByTenantAsync(Guid id)
    {
        logger.LogInformation("Getting invitations by tenant {id}", id);
        return await context.Invitations
            .Where(u => u.TenantId == id)
            .ToListAsync();
    }
}
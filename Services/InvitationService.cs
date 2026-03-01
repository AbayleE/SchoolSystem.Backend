using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Invitations;
using SchoolSystem.Backend.Exceptions;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class InvitationService(
    SchoolDbContext context,
    EmailService emailService,
    NotificationService notificationService,
    IConfiguration config,
    ILogger<InvitationService> logger,
    TenantRepository<Invitation> repo) : BaseService<Invitation>(repo)
{
    public async Task<Invitation> SendInvitationAsync(CreateInvitationDto dto, Guid tenantId, Guid senderUserId)
    {
        var sender = await context.Users.FindAsync(senderUserId)?? throw new NotFoundException("Sender not found");
       
        var existingActive = await context.Invitations
            .AnyAsync(i =>
                i.TenantId == tenantId &&
                i.Email == dto.Email &&
                i.Role == dto.Role &&
                !i.Used &&
                i.ExpiresAt > DateTime.UtcNow);
        
        if (existingActive)
            throw new InvalidOperationException($"An active invitation for {dto.Email} with role {dto.Role} already exists.");

        var invitation = new Invitation
        {
            TenantId = tenantId,
            Email = dto.Email,
            Role = dto.Role,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Used = false,
            SentByUserId = senderUserId,
        };

        await AddAsync(invitation);

        var registrationLink = $"{config["App:BaseUrl"]}/pages/register.html?token={invitation.Token}";
        await emailService.SendInvitationEmailAsync(sender.Email!,invitation, registrationLink);
       
        // await notificationService.CreateInvitationNotificationAsync(invitation);
        logger.LogInformation("Invitation sent to {Email} for role {Role} by user {SenderId}",
            dto.Email, dto.Role, senderUserId);

        return invitation;
    }


    public async Task<Invitation?> GetInvitationByTokenAsync(string token)
    {
        logger.LogInformation("Getting invitation by token {token}", token);
        return await context.Invitations
            .FirstOrDefaultAsync(i => i.Token == token && !i.Used && i.ExpiresAt > DateTime.UtcNow);
    }

    public Task<List<Invitation>> GetAllInvitationsAsync() => GetAllAsync();

    public async Task RevokeInvitationAsync(Guid invitationId)
    {
        var invitation = await GetByIdAsync(invitationId)
                         ?? throw new NotFoundException("Invitation not found.");

        if (invitation.Used)
            throw new InvalidOperationException("Cannot revoke an invitation that has already been used.");

        await DeleteAsync(invitationId);

        logger.LogInformation("Invitation {InvitationId} revoked", invitationId);
    }
}
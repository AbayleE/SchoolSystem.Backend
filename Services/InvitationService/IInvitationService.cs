using SchoolSystem.Backend.DTOs.Invitations;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Interface;

public interface IInvitationService
{
    Task<Invitation> CreateInvitationAsync(CreateInvitationDto dto);
    Task<Invitation?> GetInvitationByTokenAsync(string token);
    Task<Invitation?> GetInvitationByIdAsync(Guid id);
    Task<List<Invitation>> GetInvitationsByTenantAsync(Guid id);
}
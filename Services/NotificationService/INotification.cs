using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Interface;

public interface INotificationService
{
    Task CreateInvitationNotificationAsync(Invitation invitation);
}
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Notifications;

public class SendNotificationDto
{
    public required Guid UserId { get; set; }
    public required string Subject { get; set; }
    public required string Message { get; set; }
    public required NotificationType Type { get; set; }
}
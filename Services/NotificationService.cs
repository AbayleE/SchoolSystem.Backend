using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class NotificationService(
    TenantRepository<Notification> repo,
    ITenantContext tenant)
    : BaseService<Notification>(repo)
{
    public Task NotifyTeacherAsync(Guid teacherId, string subject, string message) =>
        CreateNotificationAsync(teacherId, subject, message, NotificationType.Assignment);

    public Task NotifyStudentAsync(Guid studentId, string subject, string message) =>
        CreateNotificationAsync(studentId, subject, message, NotificationType.Grade);

    public Task NotifyUserAsync(Guid userId, string subject, string message, NotificationType type) =>
        CreateNotificationAsync(userId, subject, message, type);
    public async Task NotifyTeacher(Guid teacherId, string message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            UserId = teacherId,
            Subject = "Assignment submission",
            Message = message,
            Type = NotificationType.Announcement,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await AddAsync(notification);
    }

    private async Task CreateNotificationAsync(
        Guid userId,
        string subject,
        string message,
        NotificationType type)
    {
        var notification = new Notification
        {
            TenantId = tenant.TenantId,
            UserId = userId,
            Subject = subject,
            Message = message,
            Type = type,
            Channel = NotificationChannel.InApp,
            IsRead = false
        };
        await AddAsync(notification);
    }
}
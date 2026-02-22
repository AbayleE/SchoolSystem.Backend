using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services.BaseService;
using SchoolSystem.Domain.Entities;
using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.Services;

public class NotificationService(
    BaseRepository<Notification> repo,
    ITenantContext tenant)
    : BaseService<Notification>(repo)
{
    private readonly ITenantContext _tenant = tenant;

    public async Task NotifyTeacher(Guid teacherId, string message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            UserId = teacherId,
            Title = "Assignment submission",
            Body = message,
            Type = NotificationType.Announcement,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await AddAsync(notification);
    }

    public async Task NotifyStudent(Guid studentId, string message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            UserId = studentId,
            Title = "Assignment graded",
            Body = message,
            Type = NotificationType.GradePublished,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await AddAsync(notification);
    }
}
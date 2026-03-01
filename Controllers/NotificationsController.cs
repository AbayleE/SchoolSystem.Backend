using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Backend.DTOs.Notifications;
using SchoolSystem.Backend.Exceptions;
using SchoolSystem.Backend.Extensions;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Backend.Services;

namespace SchoolSystem.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(NotificationService service, SchoolDbContext context, ITenantContext tenant)
    : ControllerBase
{
    // GET /api/notifications — current user's notifications
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = User.GetUserId();
        var notifications = await context.Notifications
            .Where(n => n.UserId == userId && n.TenantId == tenant.TenantId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return Ok(notifications);
    }

    // GET /api/notifications/unread — unread count
    [Authorize]
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.GetUserId();
        var count = await context.Notifications
            .CountAsync(n =>
                n.UserId == userId &&
                n.TenantId == tenant.TenantId &&
                !n.IsRead &&
                !n.IsDeleted);
        return Ok(new { unreadCount = count });
    }

    // PUT /api/notifications/{id}/read — mark one as read
    [Authorize]
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = User.GetUserId();
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == id &&
                n.UserId == userId &&
                n.TenantId == tenant.TenantId &&
                !n.IsDeleted)
            ?? throw new NotFoundException("Notification not found.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Ok(new { message = "Notification marked as read" });
    }

    // PUT /api/notifications/read-all — mark all as read
    [Authorize]
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.GetUserId();
        var unread = await context.Notifications
            .Where(n =>
                n.UserId == userId &&
                n.TenantId == tenant.TenantId &&
                !n.IsRead &&
                !n.IsDeleted)
            .ToListAsync();

        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = DateTime.UtcNow;
        }
        await context.SaveChangesAsync();

        return Ok(new { message = $"{unread.Count} notifications marked as read" });
    }

    // POST /api/notifications/send — admin sends notification to a user
    [Authorize(Roles = "SystemOwner, SchoolAdmin, Manager")]
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto)
    {
        await service.NotifyUserAsync(dto.UserId, dto.Subject, dto.Message, dto.Type);
        return Ok(new { message = "Notification sent" });
    }
}
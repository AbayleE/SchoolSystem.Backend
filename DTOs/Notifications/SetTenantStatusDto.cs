using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Notifications;

public class SetTenantStatusDto
{
    public required TenantStatus Status { get; set; }
}
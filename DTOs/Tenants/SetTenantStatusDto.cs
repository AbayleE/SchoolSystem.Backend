namespace SchoolSystem.Backend.DTOs.Tenants;

using SchoolSystem.Domain.Enums;

public class SetTenantStatusDto
{
    public required TenantStatus Status { get; set; }
}


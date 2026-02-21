using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Auth;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public required  string Role { get; set; }
    public required string Token { get; set; }
}
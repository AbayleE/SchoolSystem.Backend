using SchoolSystem.Domain.Enums;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public UserRole Role { get; set; }
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
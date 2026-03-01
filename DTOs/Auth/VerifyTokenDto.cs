namespace SchoolSystem.Backend.DTOs.Auth;

public class VerifyTokenDto
{
    public required string Token { get; set; }
}

public class VerifyTokenResponseDto
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool IsValid { get; set; }
}

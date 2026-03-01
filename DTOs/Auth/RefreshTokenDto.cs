namespace SchoolSystem.Backend.DTOs.Auth;

public class RefreshTokenDto
{
    public required string Token { get; set; }
}

public class RefreshTokenResponseDto
{
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
}

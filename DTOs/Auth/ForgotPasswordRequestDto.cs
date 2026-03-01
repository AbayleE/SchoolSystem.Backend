namespace SchoolSystem.Backend.DTOs.Auth;

public class ForgotPasswordRequestDto
{
    public required string  Email { get; set; }
    public required string SchoolName { get; set; }
}
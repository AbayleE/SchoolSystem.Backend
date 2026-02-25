namespace SchoolSystem.Backend.DTOs.Users;

public class ForgotPasswordRequestDto
{
    public required string  Email { get; set; }
    public required string NewPassword { get; set; }
}
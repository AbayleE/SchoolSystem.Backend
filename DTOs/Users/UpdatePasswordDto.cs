namespace SchoolSystem.Backend.DTOs.Users;

public class UpdatePasswordDto
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
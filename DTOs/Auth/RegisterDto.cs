namespace SchoolSystem.Backend.DTOs.Auth;

public class RegisterWithInvitationDto
{
    public required string Token { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public required string LastName { get; set; }
}
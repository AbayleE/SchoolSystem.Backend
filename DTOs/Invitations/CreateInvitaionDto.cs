namespace SchoolSystem.Backend.DTOs.Invitations;

public class CreateInvitationDto
{
    public Guid TenantId { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; } // "Teacher", "Manager", "Parent", "Student"
    public Guid SenderId { get; set; }
    public required string SenderRole { get; set; }
    public required string SenderEmail { get; set; }
}
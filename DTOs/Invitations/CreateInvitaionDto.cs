using SchoolSystem.Domain.Enums;

namespace SchoolSystem.Backend.DTOs.Invitations;

public class CreateInvitationDto
{
  
    public required string Email { get; set; }
    public required UserRole Role { get; set; } 
}
namespace SchoolSystem.Backend.DTOs.Contact;

public class CreateContactMessageDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Subject { get; set; }
    public required string Message { get; set; }
    public required string Phone { get; set; }
}

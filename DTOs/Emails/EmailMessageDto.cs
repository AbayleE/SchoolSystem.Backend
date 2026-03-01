namespace SchoolSystem.Backend.DTOs.Emails;

public class EmailMessageDto
{
    public string? To { get; set; } 
    public string? From { get; set; } 
    public string? Subject { get; set; }
    public string? HtmlBody { get; set; }
    public string? TextBody { get; set; }
    
}
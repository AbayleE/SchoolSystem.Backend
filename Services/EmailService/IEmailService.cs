using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Interface;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendInvitationEmailAsync(string toEmail, Invitation invitation);
}
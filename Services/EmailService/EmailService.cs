using System.Net;
using System.Net.Mail;
using SchoolSystem.Backend.Interface;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services.EmailService;

public class EmailService(IConfiguration config, Logger<IEmailService> logger) : IEmailService
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        throw new NotImplementedException();
    }

    public async Task SendInvitationEmailAsync(string toEmail, Invitation invitation)
    {
        var smtpSettings = config.GetSection("Smtp");

        var client = new SmtpClient(smtpSettings["Host"])
        {
            Port = int.Parse(smtpSettings["Port"]!),
            Credentials = new NetworkCredential(
                smtpSettings["Username"],
                smtpSettings["Password"]
            ),
            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(smtpSettings["From"] ?? throw new InvalidOperationException("Email address is missing")),
            Subject = "Your School System Invitation",
            Body =
                $"You have been invited.\n\nToken: {invitation.Token}\n\nUse this link to register:\nhttp://localhost:5275/swagger/register?token={invitation.Token}",
            IsBodyHtml = false
        };

        message.To.Add(toEmail);

        await client.SendMailAsync(message);
        logger.LogInformation("Invitation email sent to {Email}", toEmail);
    }
}
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SchoolSystem.Domain.Entities;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace SchoolSystem.Backend.Services;

public class EmailService(IConfiguration config, ILogger<EmailService> logger)
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        throw new NotImplementedException();
    }

    public async Task SendInvitationEmailAsync(string fromEmail, Invitation invitation, string registrationLink)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(config["EmailSettings:FromEmail"] ?? throw new InvalidOperationException()));
        email.To.Add(MailboxAddress.Parse(invitation.Email ?? throw new InvalidOperationException()));
        
        email.Subject = "You're invited to join the School System";
        
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"""
                        <h2>You've been invited!</h2>
                        <p>You have been invited to join the School System as a <strong>{invitation.Role}</strong>.</p>
                        <p>Click the button below to complete your registration. This link expires in <strong>7 days</strong>.</p>
                        <br>
                        <a href="{registrationLink}" 
                           style="background:#4F46E5;color:white;padding:12px 24px;border-radius:6px;text-decoration:none;">
                           Accept Invitation
                        </a>
                        <p>Or copy this link into your browser:</p>
                        <p>{registrationLink}</p>
                        <p>If you weren't expecting this invitation, you can safely ignore this email.</p>
                        """,
            TextBody = $"""
                        You've been invited to join the School System as {invitation.Role}.

                        Use this link to complete your registration (expires in 7 days):
                        {registrationLink}

                        If you weren't expecting this, you can ignore this email.
                        """
        };
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        
        await smtp.ConnectAsync(config["EmailSettings:SmtpServer"],
            int.Parse(config["EmailSettings:Port"] ?? throw new InvalidOperationException()), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(config["EmailSettings:Username"], config["EmailSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        
        logger.LogInformation("Invitation email sent to {Email}",fromEmail);
    }
}
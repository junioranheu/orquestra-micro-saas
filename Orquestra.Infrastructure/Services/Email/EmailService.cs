using Orquestra.Domain.Entities;
using System.Net;
using System.Net.Mail;

namespace Orquestra.Infrastructure.Services.Email;

public class EmailService(EmailSettings settings) : IEmailService
{
    private readonly EmailSettings _settings = settings;

    public async Task SendEmail(string to, string subject, string body, bool isHtml = true, IEnumerable<string>? cc = null)
    {
        using SmtpClient client = new(_settings.SmtpHost, _settings.SmtpPort)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        MailMessage mailMessage = new()
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };

        mailMessage.To.Add(to);

        if (cc is not null && cc.Any())
        {
            foreach (var c in cc)
            {
                mailMessage.CC.Add(c);
            }
        }

        await client.SendMailAsync(mailMessage);
    }
}
using Orquestra.Domain.Entities;
using System.Net;
using System.Net.Mail;

namespace Orquestra.Infrastructure.Services.Email;

public class EmailService(EmailSettings settings) : IEmailService
{
    private readonly EmailSettings _settings = settings;
    const string _smtpHost = "smtp-relay.brevo.com";
    const int _smtpPort = 587;
    const string _senderName = "Orquestra";
    const string _senderEmail = "orquestra.saas@gmail.com";
    const string _username = "953807001@smtp-brevo.com";
    const bool _enableSsl = true;

    public async Task SendEmail(string to, string subject, string body, bool isHtml = true, IEnumerable<string>? cc = null)
    {
        using SmtpClient client = new(_smtpHost, _smtpPort)
        {
            Credentials = new NetworkCredential(_username, _settings.Password),
            EnableSsl = _enableSsl
        };

        MailMessage mailMessage = new()
        {
            From = new MailAddress(_senderName, _senderEmail),
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
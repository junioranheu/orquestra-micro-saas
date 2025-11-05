using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orquestra.Domain.Consts;
using Orquestra.Infrastructure.Services.Email.Models;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Orquestra.Infrastructure.Services.Email;

public class EmailService(EmailSettings settings, IWebHostEnvironment env) : IEmailService
{
    private readonly EmailSettings _settings = settings;
    private readonly bool _isDevelopment = env.IsDevelopment();

    public async Task SendEmail(EmailInput input)
    {
        using SmtpClient client = new(SystemConsts.Brevo.SmtpHost, SystemConsts.Brevo.SmtpPort)
        {
            Credentials = new NetworkCredential(SystemConsts.Brevo.Username, _settings.SmtpKey),
            EnableSsl = SystemConsts.Brevo.EnableSsl
        };

        if (_isDevelopment)
        {
            input.Subject = $"[DEBUG] {input.Subject}";
        }

        MailMessage mailMessage = new()
        {
            From = new MailAddress(address: SystemConsts.Brevo.SenderEmail, displayName: SystemConsts.Brevo.SenderName),
            Subject = input.Subject,
            Body = input.Body,
            IsBodyHtml = input.IsHtml,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        mailMessage.To.Add(input.To);

        if (input.Cc is not null && input.Cc.Count != 0)
        {
            foreach (var c in input.Cc)
            {
                mailMessage.CC.Add(c);
            }
        }

        mailMessage.HeadersEncoding = Encoding.UTF8;

        if (_isDevelopment && SystemConsts.Brevo.DoNotSendEmailIfDev)
        {
            return;
        }

        await client.SendMailAsync(mailMessage);
    }
}
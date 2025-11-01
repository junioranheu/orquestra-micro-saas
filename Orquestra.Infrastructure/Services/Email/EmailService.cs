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

    public async Task SendEmail(string to, string subject, string body, bool isHtml = true, List<string>? cc = null)
    {
        using SmtpClient client = new(SystemConsts.Brevo.SmtpHost, SystemConsts.Brevo.SmtpPort)
        {
            Credentials = new NetworkCredential(SystemConsts.Brevo.Username, _settings.Password),
            EnableSsl = SystemConsts.Brevo.EnableSsl
        };

        if (_isDevelopment)
        {
            subject = $"[DEBUG] {subject}";
        }

        MailMessage mailMessage = new()
        {
            From = new MailAddress(address: SystemConsts.Brevo.SenderEmail, displayName: SystemConsts.Brevo.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };

        mailMessage.To.Add(to);

        if (cc is not null && cc.Count != 0)
        {
            foreach (var c in cc)
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

    /// <summary>
    /// Carrega um arquivo de template HTML e substitui os placeholders pelos valores fornecidos.
    /// Dictionary<string, string> values = new()
    /// {
    ///    { "[UserName]", "Junior" },
    ///    { "[CompanyName]", "Orquestra" },
    ///    { "[ConfirmLink]", "orquestra.com/confirm?token=123" }
    /// };
    /// </summary>
    /// <param name="templatePath">Caminho completo do arquivo HTML do template.</param>
    /// <param name="values">Dicionário contendo os placeholders e seus respectivos valores. 
    /// Cada chave deve corresponder a um placeholder no template, por exemplo [Name].</param>
    /// <returns>Retorna uma string com o conteúdo do template já com os placeholders substituídos pelos valores.</returns>
    public string RenderTemplate(string templateName, Dictionary<string, string> values)
    {
        string basePath = Path.Combine(AppContext.BaseDirectory, "Services", "Email", "Templates");
        string templatePath = Path.Combine(basePath, templateName);

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template não encontrado: {templatePath}");
        }

        string template = File.ReadAllText(templatePath);

        // Substitui os placeholders;
        foreach (var kv in values)
        {
            template = template.Replace(kv.Key, kv.Value);
        }

        return template;
    }
}
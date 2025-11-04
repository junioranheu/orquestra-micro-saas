using Orquestra.Infrastructure.Services.Email.Models;

namespace Orquestra.Infrastructure.Services.Email;

public interface IEmailService
{
    Task SendEmail(EmailInput input);
    string RenderTemplate(string templatePath, Dictionary<string, string> values);
}
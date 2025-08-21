namespace Orquestra.Infrastructure.Services.Email;

public interface IEmailService
{
    Task SendEmail(string to, string subject, string body, bool isHtml = true, IEnumerable<string>? cc = null);
}
namespace Orquestra.Infrastructure.Services.Sms;

public interface ISmsService
{
    Task<string> SendSms(string to, string from, string text, bool mustThrow = false, string tag = "", string type = "transactional", string? callbackUrl = null);
}
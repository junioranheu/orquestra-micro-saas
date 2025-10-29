namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessage;

public interface ISendMessageWhatsApp
{
    Task Execute(Guid userIdAuth);
}
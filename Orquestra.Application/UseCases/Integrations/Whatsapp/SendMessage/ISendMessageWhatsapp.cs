using Orquestra.Application.UseCases.Integrations.Whatsapp.Shared;

namespace Orquestra.Application.UseCases.Integrations.Whatsapp.SendMessage;

public interface ISendMessageWhatsapp
{
    Task Execute(Guid userIdAuth, IntegrationWhatsappMessageInput input);
}
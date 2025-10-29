using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessage;

public sealed class SendMessageWhatsApp(IntegrationWhatsAppBaseDependencies deps) : IntegrationWhatsAppBase(deps), ISendMessageWhatsApp
{
    public async Task Execute(Guid userIdAuth)
    {
        await Validate(userIdAuth);

        // TO DO;
    }
}
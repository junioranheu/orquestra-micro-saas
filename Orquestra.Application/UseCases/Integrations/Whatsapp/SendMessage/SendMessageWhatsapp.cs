using Orquestra.Application.UseCases.Integrations.Whatsapp.Base;

namespace Orquestra.Application.UseCases.Integrations.Whatsapp.SendMessage;

public sealed class SendMessageWhatsapp(IntegrationWhatsappBaseDependencies deps) : IntegrationWhatsappBase(deps), ISendMessageWhatsapp
{
    public async Task Execute(Guid userIdAuth)
    {
        await Validate(userIdAuth);

        // TO DO;
    }
}
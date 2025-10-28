using Orquestra.Application.UseCases.Integrations.Whatsapp.Base;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Shared;

namespace Orquestra.Application.UseCases.Integrations.Whatsapp.SendMessage;

public sealed class SendMessageWhatsapp(IntegrationWhatsappBaseDependencies deps) : IntegrationWhatsappBase(deps), ISendMessageWhatsapp
{
    public async Task Execute(Guid userIdAuth, IntegrationWhatsappMessageInput input)
    {
        await Validate(userIdAuth, input);

        // TO DO;
    }
}
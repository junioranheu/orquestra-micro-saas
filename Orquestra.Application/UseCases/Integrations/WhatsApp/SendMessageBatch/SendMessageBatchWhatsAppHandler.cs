using Orquestra.Infrastructure.Jobs.Base.Handlers;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;

public class SendMessageBatchWhatsAppHandler(ISendMessageBatchWhatsApp sendMessageBatchWhatsApp) : ISendMessageBatchWhatsAppHandler
{
    private readonly ISendMessageBatchWhatsApp _sendMessageBatchWhatsApp = sendMessageBatchWhatsApp;

    public async Task ExecuteAsync(CancellationToken token)
    {
        await _sendMessageBatchWhatsApp.Execute(token);
    }
}
namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;

public interface ISendMessageBatchWhatsApp
{
    Task<int> Execute(CancellationToken token);
}
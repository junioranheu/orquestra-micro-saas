namespace Orquestra.Infrastructure.Jobs.Base.Handlers;

public interface ISendMessageBatchWhatsAppHandler
{
    Task ExecuteAsync(CancellationToken token);
}
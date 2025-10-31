namespace Orquestra.Infrastructure.Jobs.Base.Handlers;

public interface ISendMessageBatchWhatsAppHandler
{
    Task<int> ExecuteAsync(CancellationToken token);
}
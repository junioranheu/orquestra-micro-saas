using Orquestra.Infrastructure.Factory.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Orquestra.Infrastructure.Messaging.Consumers;

public class GenericConsumer<TMessage>(
    IRabbitMQConnection connection,
    string queueName,
    Func<TMessage, CancellationToken, Task> handleMessage,
    ushort prefetchCount = 1,
    Func<Exception, bool>? shouldRequeue = null) : IGenericConsumer
{
    #region constructor
    private readonly IRabbitMQConnection _connection = connection;
    private readonly string _queueName = queueName;
    private readonly Func<TMessage, CancellationToken, Task> _handleMessage = handleMessage;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ushort _prefetchCount = prefetchCount;
    private readonly Func<Exception, bool> _shouldRequeue = shouldRequeue ?? new Func<Exception, bool>(ex => ex is TimeoutException || ex is HttpRequestException || ex is IOException || ex is TaskCanceledException || ex is OperationCanceledException || ex is SocketException);
    #endregion

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        IChannel channel = _connection.Channel;

        await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _prefetchCount, global: false, cancellationToken: cancellationToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, x) =>
        {
            try
            {
                string body = Encoding.UTF8.GetString(x.Body.ToArray());
                TMessage? message = JsonSerializer.Deserialize<TMessage>(body, _jsonOptions);

                if (message is null)
                {
                    // Se a mensagem estiver inválida, marca como ACK pra não ficar em loop
                    await channel.BasicAckAsync(deliveryTag: x.DeliveryTag, multiple: false);
                    return;
                }

                // Executa o handler;
                await _handleMessage(message, cancellationToken);

                await channel.BasicAckAsync(deliveryTag: x.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                bool requeue = _shouldRequeue(ex);
                await channel.BasicNackAsync(deliveryTag: x.DeliveryTag, multiple: false, requeue: requeue);
            }
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer, cancellationToken);
    }
}
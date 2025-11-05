using Orquestra.Infrastructure.Factory.RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Orquestra.Infrastructure.Messaging.Consumers;

public class GenericConsumer<TMessage> : IGenericConsumer
{
    #region constructor
    private readonly IRabbitMQConnection _connection;
    private readonly string _queueName;
    private readonly Func<TMessage, CancellationToken, Task> _handleMessage;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ushort _prefetchCount;
    private readonly Func<Exception, bool> _shouldRequeue;

    public GenericConsumer(
        IRabbitMQConnection connection,
        string queueName,
        Func<TMessage, CancellationToken, Task> handleMessage,
        JsonSerializerOptions? jsonOptions = null,
        ushort prefetchCount = 1,
        Func<Exception, bool>? shouldRequeue = null)
    {
        _connection = connection;
        _queueName = queueName;
        _handleMessage = handleMessage;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _prefetchCount = prefetchCount;
        _shouldRequeue = shouldRequeue ?? new Func<Exception, bool>(ex => ex is TimeoutException || ex is HttpRequestException || ex is IOException || ex is TaskCanceledException || ex is OperationCanceledException || ex is SocketException);
    }
    #endregion

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        IChannel channel = _connection.Channel;

        await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _prefetchCount, global: false, cancellationToken: cancellationToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                string body = Encoding.UTF8.GetString(ea.Body.ToArray());
                TMessage? message = JsonSerializer.Deserialize<TMessage>(body, _jsonOptions);

                if (message is null)
                {
                    // Se a mensagem estiver inválida, marca como ACK pra não ficar em loop
                    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }

                // Executa o handler;
                await _handleMessage(message, cancellationToken);

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                bool requeue = _shouldRequeue(ex);
                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: requeue);
            }
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer, cancellationToken);
    }
}
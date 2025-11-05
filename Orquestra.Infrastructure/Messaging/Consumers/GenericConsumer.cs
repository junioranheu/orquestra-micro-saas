using Microsoft.Extensions.Logging;
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
    ILogger<GenericConsumer<TMessage>> logger,
    ushort prefetchCount = 1,
    Func<Exception, bool>? shouldRequeue = null) : IAsyncDisposable
{
    #region constructor
    private readonly IRabbitMQConnection _connection = connection;
    private readonly string _queueName = queueName;
    private readonly Func<TMessage, CancellationToken, Task> _handleMessage = handleMessage;
    private readonly ILogger<GenericConsumer<TMessage>> _logger = logger;
    private readonly ushort _prefetchCount = prefetchCount;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IChannel? _channel;
    private string? _consumerTag;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Func<Exception, bool> _shouldRequeue = shouldRequeue ?? (ex => ex is TimeoutException or HttpRequestException or IOException or TaskCanceledException or OperationCanceledException or SocketException);
    #endregion

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (_channel is not null)
            {
                _logger.LogWarning("Consumer já iniciado para a fila {Queue}", _queueName);
                return;
            }

            _channel = _connection.Channel;

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: _prefetchCount,
                global: false,
                cancellationToken: cancellationToken
            );

            AsyncEventingBasicConsumer consumer = new(_channel);
            consumer.ReceivedAsync += OnMessageReceived;

            _consumerTag = await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Consumer iniciado com sucesso para a fila {Queue} com prefetch {Prefetch}", _queueName, _prefetchCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar consumer para a fila {Queue}", _queueName);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
    {
        ulong deliveryTag = eventArgs.DeliveryTag;

        try
        {
            string body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            _logger.LogDebug("Mensagem recebida da fila {Queue} - DeliveryTag: {DeliveryTag}", _queueName, deliveryTag);

            TMessage? message = JsonSerializer.Deserialize<TMessage>(body, _jsonOptions);

            if (message is null)
            {
                _logger.LogWarning("Mensagem inválida recebida na fila {Queue} - DeliveryTag: {DeliveryTag}. Body: {Body}", _queueName, deliveryTag, body);
                await _channel!.BasicAckAsync(deliveryTag, multiple: false);
                return;
            }

            // Handler para o método dinâmico passando no DI;
            await _handleMessage(message, CancellationToken.None);
            await _channel!.BasicAckAsync(deliveryTag, multiple: false);

            _logger.LogDebug("Mensagem processada com sucesso na fila {Queue} - DeliveryTag: {DeliveryTag}", _queueName, deliveryTag);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "Erro ao deserializar mensagem da fila {Queue} - DeliveryTag: {DeliveryTag}", _queueName, deliveryTag);

            // Mensagem malformada, ACK para não reprocessar;
            await _channel!.BasicAckAsync(deliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            bool requeue = _shouldRequeue(ex);
            _logger.LogError(ex, "Erro ao processar mensagem da fila {Queue} - DeliveryTag: {DeliveryTag}. Requeue: {Requeue}", _queueName, deliveryTag, requeue);

            await _channel!.BasicNackAsync(
                deliveryTag: deliveryTag,
                multiple: false,
                requeue: requeue
            );

            if (!requeue)
            {
                _logger.LogWarning("Mensagem descartada (não será reprocessada) da fila {Queue} - DeliveryTag: {DeliveryTag}. " + "Considere implementar Dead Letter Queue para mensagens com falha permanente.", _queueName, deliveryTag);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null && !string.IsNullOrEmpty(_consumerTag))
        {
            try
            {
                _logger.LogInformation("Cancelando consumer da fila {Queue} - ConsumerTag: {ConsumerTag}", _queueName, _consumerTag);
                await _channel.BasicCancelAsync(_consumerTag);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao cancelar consumer da fila {Queue}", _queueName);
            }
        }

        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}
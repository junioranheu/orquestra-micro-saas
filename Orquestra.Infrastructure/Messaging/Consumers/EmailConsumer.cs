using Orquestra.Infrastructure.Factory.RabbitMQ;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Orquestra.Infrastructure.Messaging.Consumers;

public class EmailConsumer(IRabbitMQConnection connection, IEmailService emailService, string queueName)
{
    private readonly IRabbitMQConnection _connection = connection;
    private readonly IEmailService _emailService = emailService;
    private readonly string _queueName = queueName;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        IChannel channel = _connection.Channel;

        await channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken); // Define quantas mensagens são enviadas por "vôo";
        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, x) =>
        {
            try
            {
                string message = Encoding.UTF8.GetString(x.Body.ToArray());
                await ProcessEmailAsync(message);
                await channel.BasicAckAsync(deliveryTag: x.DeliveryTag, multiple: false); // Terminou de processar a mensagem com sucesso;
            }
            catch (Exception ex)
            {
                bool shouldRequeue = ex is TimeoutException || ex is HttpRequestException || ex is IOException || ex is TaskCanceledException || ex is OperationCanceledException || ex is SocketException;
                await channel.BasicNackAsync(deliveryTag: x.DeliveryTag, multiple: false, requeue: shouldRequeue); // Não foi possível processar a mensagem. É definido para reentrar na fila;
            }
        };

        await channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer, cancellationToken);
    }

    private async Task ProcessEmailAsync(string message)
    {
        EmailInput? email = JsonSerializer.Deserialize<EmailInput>(message);

        if (email is null)
        {
            return;
        }

        await _emailService.SendEmail(email);
    }
}
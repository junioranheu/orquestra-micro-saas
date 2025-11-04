using Orquestra.Infrastructure.Factory.RabbitMQ;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Email.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
        await channel.BasicQosAsync(0, 1, false, cancellationToken);

        AsyncEventingBasicConsumer consumer = new(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await ProcessEmailAsync(message);
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch
            {
                await channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await channel.BasicConsumeAsync(_queueName, autoAck: false, consumer, cancellationToken);
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
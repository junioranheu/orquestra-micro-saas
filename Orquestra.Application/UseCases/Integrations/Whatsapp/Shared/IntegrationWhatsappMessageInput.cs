namespace Orquestra.Application.UseCases.Integrations.Whatsapp.Shared;

public sealed class IntegrationWhatsappMessageInput
{
    public required string Phone { get; set; }

    public required string Message { get; set; }

    public required string From { get; set; }

    public DateTime DateEnd { get; set; }
}
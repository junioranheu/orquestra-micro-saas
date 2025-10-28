using static Orquestra.Utils.Fixtures.Get;


namespace Orquestra.Application.UseCases.Integrations.Whatsapp.Shared;

public sealed class IntegrationWhatsappMessageInput
{
    public required string Phone { get; set; }

    public required string Message { get; set; }

    public required string FromFullName { get; set; }

    public string? CompanyName { get; set; } = string.Empty;

    public DateTime Date { get; set; } = GetDate();
}
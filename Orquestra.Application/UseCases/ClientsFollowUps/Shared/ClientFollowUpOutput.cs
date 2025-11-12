using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Shared;

public sealed class ClientFollowUpOutput
{
    public Guid ClientFollowUpId { get; set; }
    public Guid ClientId { get; set; }
    public string? Observation { get; set; }
    public ClientFollowUpStatus ClientFollowUpStatus { get; set; }
    public List<string> ImagesBase64 { get; set; } = []; // Base64;
    public List<string>? ImagesContentType { get; set; } = [];
}
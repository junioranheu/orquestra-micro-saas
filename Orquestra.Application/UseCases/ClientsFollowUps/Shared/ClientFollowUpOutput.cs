using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Shared;

public sealed class ClientFollowUpOutput
{
    public Guid ClientFollowUpId { get; set; }
    public Guid ClientId { get; set; }
    public ClientOutput? Client { get; set; }
    public Guid CompanyId { get; set; }
    public string? Observation { get; set; }
    public ClientFollowUpStatusEnum ClientFollowUpStatus { get; set; }
    public List<string> ImagesBase64 { get; set; } = []; // Base64;
    public DateTime? CreatedDate { get; set; }  
}
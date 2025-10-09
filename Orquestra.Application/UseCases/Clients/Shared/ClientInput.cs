using System.Text.Json.Serialization;

namespace Orquestra.Application.UseCases.Clients.Shared;

public sealed class ClientInput
{
    [JsonIgnore]
    public Guid? ClientId { get; set; }

    public string? FullName { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;

    public string? CPF { get; set; } = string.Empty;

    public string? Address { get; set; } = string.Empty;

    public DateTime? DateOfBirth { get; set; }

    public string? Phone { get; set; } = string.Empty;

    public string? Notes { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }
}
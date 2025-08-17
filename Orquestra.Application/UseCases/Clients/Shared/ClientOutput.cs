using Orquestra.Domain.Entities;
using System.Text.Json.Serialization;

namespace Orquestra.Application.UseCases.Clients.Shared;

public sealed class ClientOutput
{
    public Guid ClientId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string CPF { get; set; } = string.Empty;

    public string? Address { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Notes { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }
    [JsonIgnore]
    public Company? Companies { get; set; }
}
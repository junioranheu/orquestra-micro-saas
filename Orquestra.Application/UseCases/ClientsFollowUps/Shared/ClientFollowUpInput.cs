using Microsoft.AspNetCore.Http;
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.ClientsFollowUps.Shared;

public sealed class ClientFollowUpInput
{
    public Guid? ClientFollowUpId { get; set; }
    public Guid? ClientId { get; set; }
    public string? Observation { get; set; }
    public ClientFollowUpStatus ClientFollowUpStatus { get; set; }
    public List<IFormFile> ImagesFormFile { get; set; } = [];
}
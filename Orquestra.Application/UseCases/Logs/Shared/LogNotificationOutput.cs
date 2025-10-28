namespace Orquestra.Application.UseCases.Logs.Shared;

public sealed class LogNotificationOutput
{
    public Guid LogId { get; set; }
    public string Emoji { get; set; } = string.Empty;
    public string? LogType { get; set; } = string.Empty;
    public string? RequestType { get; set; } = string.Empty;
    public string? EndpointName { get; set; } = string.Empty;
    public string? RawEndpoint { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Story { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public override string ToString()
    {
        return $"{Emoji} [{LogType}] {RequestType} — {EndpointName}";
    }
}
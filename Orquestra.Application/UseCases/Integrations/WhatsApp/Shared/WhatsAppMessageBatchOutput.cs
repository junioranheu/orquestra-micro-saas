using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.Shared;

public sealed class WhatsAppMessageBatchOutput
{
    public required string CompanyName { get; set; } = string.Empty;

    public required string ClientName { get; set; } = string.Empty;
    public required string ClientPhone { get; set; } = string.Empty;

    public required DateTime ScheduleDate { get; set; }
    public required ScheduleStatusEnum ScheduleStatus { get; set; } 

    public required string MessageReminderBeforeSchedule { get; set; } = string.Empty;
    public required string MessageBeforeScheduleAlert { get; set; } = string.Empty;
    public required string MessageOnScheduleConfirmed { get; set; } = string.Empty;
    public required string MessageOnScheduleCanceled { get; set; } = string.Empty;
}
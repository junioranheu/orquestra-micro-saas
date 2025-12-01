using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleInput
{
    public Guid? ScheduleId { get; set; }

    public DateTime DateStart { get; set; }

    public string TimeStart => DateStart.ToString("HH:mm");

    public DateTime DateEnd { get; set; }

    public string TimeEnd => DateEnd.ToString("HH:mm");

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public ScheduleTypeEnum ScheduleType { get; set; }

    public Guid ClientId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid[]? UsersIds { get; set; } = [];

    public bool IsRestrictForSpecificUsers { get; set; }

    public string? CustomTitle { get; set; }

    public string? CustomUrl { get; set; }

    public string? Observation { get; set; }

    public decimal? AmountReceived { get; set; }
}
using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleInput
{
    public Guid? ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public int DurationMinutes { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid[]? UsersIds { get; set; } = [];

    public bool IsRestrictForSpecificUsers { get; set; }

    public string? CustomTitle { get; set; }

    public string? CustomUrl { get; set; }

    public string? Observation { get; set; }

    public decimal? AmountReceived { get; set; }

    // Extras;
    public DateTime DateEnd { get; set; }
}
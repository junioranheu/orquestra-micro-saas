using Orquestra.Domain.Enums;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleInput
{
    public Guid? ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid[]? Users { get; set; } = [];
}
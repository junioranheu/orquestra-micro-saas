using Orquestra.Domain.Enums;
using System.Text.Json.Serialization;

namespace Orquestra.Application.UseCases.Schedules.Shared;

public sealed class ScheduleInput
{
    [JsonIgnore]
    public Guid? ScheduleId { get; set; }

    public DateTime Date { get; set; }

    public PaymentTypeEnum PaymentType { get; set; }

    public ScheduleStatusEnum ScheduleStatus { get; set; }

    public Guid ClientId { get; set; }

    public Guid CompanyId { get; set; }

    public Guid[]? UsersIds { get; set; } = [];

    public bool IsRestrictForSpecificUsers { get; set; }
}
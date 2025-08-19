using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ScheduleStatusEnum
{
    [Description("Marcado")]
    Scheduled = 1,

    [Description("Remarcado")]
    Rescheduled = 2,

    [Description("Concluído")]
    Completed = 3,

    [Description("Cancelado")]
    Cancelled = 4
}
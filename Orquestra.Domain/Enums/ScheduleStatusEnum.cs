using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ScheduleStatusEnum
{
    [Description("Marcado")]
    Scheduled = 1,

    [Description("Concluído")]
    Completed = 2,

    [Description("Cancelado")]
    Canceled = 3
}
using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ClientFollowUpStatusEnum
{
    [Description("Em andamento")]
    InProgress = 1,

    [Description("Finalizado")]
    Completed = 2,

    [Description("Cancelado")]
    Cancelled = 3
}
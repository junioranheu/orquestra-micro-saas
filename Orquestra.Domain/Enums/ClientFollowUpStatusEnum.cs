using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ClientFollowUpStatus
{
    [Description("Pendente")]
    Pending = 1,

    [Description("Em progresso")]
    InProgress = 2,

    [Description("Finalizado")]
    Completed = 3,

    [Description("Cancelado")]
    Cancelled = 4
}
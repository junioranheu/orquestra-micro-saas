using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ServiceOrderStatusEnum
{
    [Description("Pendente")]
    Pending = 1,

    [Description("Em progresso")]
    InProgress = 2,

    [Description("Finalizado")]
    Completed = 3,

    [Description("Cancelado")]
    Canceled = 4
}

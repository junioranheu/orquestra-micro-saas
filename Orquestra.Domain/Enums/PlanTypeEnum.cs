using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum PlanTypeEnum
{
    [Description("Basico")]
    Basico = 1,

    [Description("Premium")]
    Premium = 2
}
using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum PlanTypeEnum
{
    [Description("Básico")]
    Basic = 1,

    [Description("Premium")]
    Premium = 2
}
using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum PlanTypeEnum
{
    [Description("Básico")]
    Basico = 1,

    [Description("Premium")]
    Premium = 2
}
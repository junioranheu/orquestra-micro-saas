using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Agenda")]
    Scheduling = 1,

    [Description("Módulo financeiro")]
    Sales = 2
}
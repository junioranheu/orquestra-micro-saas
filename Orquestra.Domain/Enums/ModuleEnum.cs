using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Módulo de controle de agendamentos")]
    Scheduling = 1,

    [Description("Módulo de controle financeiro")]
    Financial = 2
}
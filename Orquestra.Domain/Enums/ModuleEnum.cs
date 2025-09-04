using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Módulo de controle de agendamentos")]
    Agendamento = 1,

    [Description("Módulo de controle financeiro")]
    Financeiro = 2
}
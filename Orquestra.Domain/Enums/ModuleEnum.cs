using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Agenda")]
    Scheduling = 1,

    [Description("Integração com WhatsApp")]
    IntegrationWhatsApp = 2,

    [Description("Follow-up")]
    CostumerFollowUp = 3,

    [Description("Nota fiscal")]
    Invoice = 4,

    [Description("Financeiro")]
    Sales = 5,
}
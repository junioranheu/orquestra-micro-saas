using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Agenda")]
    Scheduling = 1,

    [Description("Integração com WhatsApp")]
    IntegrationWhatsApp = 2,

    [Description("Acompanhamento")]
    ClientFollowUp = 3,

    [Description("Nota fiscal")]
    Invoice = 4,

    [Description("Financeiro")]
    Sales = 5,

    [Description("Orçamento")]
    Quote = 6,

    [Description("Ordem de serviço")]
    ServiceOrder = 7,

    [Description("Estoque")]
    Inventory = 8
}
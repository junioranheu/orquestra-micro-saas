using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Colaboradores")]
    Members = 1,

    [Description("Clientes")]
    Clients = 2,

    [Description("Agenda")]
    Scheduling = 3,

    [Description("Integração com WhatsApp")]
    IntegrationWhatsApp = 4,

    [Description("Acompanhamento")]
    ClientFollowUp = 5,

    [Description("Nota fiscal")]
    Invoice = 6,

    [Description("Financeiro")]
    Sales = 7,

    [Description("Orçamento")]
    Quote = 8,

    [Description("Ordem de serviço")]
    ServiceOrder = 9,

    [Description("Estoque")]
    Inventory = 10
}
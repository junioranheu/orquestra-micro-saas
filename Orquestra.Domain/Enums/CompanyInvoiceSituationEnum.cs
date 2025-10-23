using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanyInvoiceSituationEnum
{
    [Description("Pendente")]
    Pending = 1,

    [Description("Aprovado")]
    Paid = 2,

    [Description("Vencido")]
    Expired = 3,

    [Description("Cancelado")]
    Canceled = 999
}
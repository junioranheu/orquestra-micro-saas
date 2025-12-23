using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanySituationEnum
{
    [Description("Pagamento pendente")]
    PendingPayment = 1,

    [Description("Aprovado")]
    Approved = 2
}
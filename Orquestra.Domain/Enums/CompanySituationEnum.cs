using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanySituationEnum
{
    // [Description("Módulo(s) comprado(s), mas pagamento pendente")]
    [Description("Pagamento pendente")]
    PendingPayment = 1,

    // [Description("Módulo(s) comprado(s)")]
    [Description("Aprovado")]
    Approved = 2
}
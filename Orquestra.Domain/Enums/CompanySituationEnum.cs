using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanySituationEnum
{
    // [Description("Registrado na plataforma, mas ainda sem nenhum módulo adquirido")]
    [Description("Nenhum módulo adquirido")]
    RegisteredButWithoutAnyModules = 1,

    // [Description("Módulo(s) comprado(s), mas pagamento pendente")]
    [Description("Pagamento pendente")]
    PendingPayment = 2,

    // [Description("Módulo(s) comprado(s)")]
    [Description("Aprovado")]
    Approved = 3
}
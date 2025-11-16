using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum QuoteStatusEnum
{
    [Description("Rascunho")]
    Draft = 1,

    [Description("Enviado ao cliente")]
    Sent = 2,

    [Description("Aprovado pelo cliente")]
    Approved = 3,

    [Description("Recusado pelo cliente")]
    Rejected = 4,

    [Description("Expirado")]
    Expired = 5
}
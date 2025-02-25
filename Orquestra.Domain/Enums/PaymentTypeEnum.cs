using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum PaymentTypeEnum
{
    [Description("Dinheiro")]
    Dinheiro = 1,

    [Description("Crédito")]
    Credito = 2,

    [Description("Débito")]
    Debito = 3,

    [Description("Pix")]
    Pix = 4,

    [Description("TED")]
    TED = 5,

    [Description("Outro")]
    Outro = 6
}
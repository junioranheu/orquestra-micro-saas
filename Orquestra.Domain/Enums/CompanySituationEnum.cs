using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanySituationEnum
{
    [Description("Registrado mas ainda não aprovado")]
    RegisteredButNotApproved = 1,

    [Description("Aprovado")]
    Approved = 2,

    [Description("Aprovado mas a mensalidade não foi paga")]
    ApprovedButNotPaid = 3
}
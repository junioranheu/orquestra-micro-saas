using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum UserRoleEnum
{
    [Description("Usuário")]
    Common = 1,

    [Description("Suporte")]
    Maintainer = 999
}
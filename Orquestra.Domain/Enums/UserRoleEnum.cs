using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum UserRoleEnum
{
    [Description("Usuário do sistema")]
    Common = 1,

    [Description("Suporte do sistema")]
    Maintainer = 999,

    [Description("Administrador do sistema")]
    Admin = 1000
}
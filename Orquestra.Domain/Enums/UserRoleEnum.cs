using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum UserRoleEnum
{
    [Description("Administrador")]
    Administrador = 1,

    [Description("Usuário")]
    Comum = 2,

    [Description("Suporte")]
    Suporte = 9
}

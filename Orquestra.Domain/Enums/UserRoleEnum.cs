using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum UserRoleEnum
{
    [Description("Usuário")]
    Comum = 1,

    [Description("Suporte")]
    Suporte = 999
}
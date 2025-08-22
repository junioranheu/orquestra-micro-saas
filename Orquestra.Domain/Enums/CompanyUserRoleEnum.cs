using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanyUserRoleEnum
{
    [Description("Proprietário")]
    Owner = 1,

    [Description("Administrador")]
    Administrator = 2,

    [Description("Membro")]
    Member = 3
}
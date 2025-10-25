using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum CompanyUserRoleEnum
{
    [Description("Administrador")]
    Administrator = 1,

    [Description("Colaborador")]
    Member = 2
}
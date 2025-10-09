using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum LogTypeEnum
{
    [Description("Exceção")]
    Exception = 1,

    [Description("Requisição")]
    Request = 2
}
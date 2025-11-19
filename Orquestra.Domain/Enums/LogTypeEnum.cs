using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum LogTypeEnum
{
    [Description("Exceção")]
    Exception = 1,

    [Description("Requisição")]
    Request = 2,

    [Description("Job")]
    Job = 3,

    [Description("Auditoria")]
    Audit = 4
}
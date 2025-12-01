using System.ComponentModel;

namespace Orquestra.Domain.Enums;

public enum ScheduleTypeEnum
{
    [Description("Consulta")]
    Consulta = 1,

    [Description("Manutenção")]
    Manutencao = 2,

    [Description("Cirurgia")]
    Cirurgia = 3,

    [Description("Implante")]
    Implante = 4,

    [Description("Avaliação")]
    Avaliacao = 5,

    [Description("Retorno")]
    Retorno = 6,

    [Description("Urgência")]
    Urgencia = 7,

    [Description("Limpeza")]
    Limpeza = 8,

    [Description("Procedimento")]
    Procedimento = 9,

    [Description("Acompanhamento pós-operatório")]
    AcompanhamentoPosOperatorio = 10,

    [Description("Check-up")]
    CheckUp = 11,

    [Description("Orientação")]
    Orientacao = 12,

    [Description("Prótese")]
    Protese = 13,

    [Description("Exame")]
    Exame = 14,

    [Description("Ajuste")]
    Ajuste = 15,

    [Description("Aplicação")]
    Aplicacao = 16,

    [Description("Sessão")]
    Sessao = 17
}
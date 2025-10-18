using Orquestra.Domain.Consts;
using System.ComponentModel;
using System.Reflection;

namespace Orquestra.Domain.Enums;

public enum PlanTypeEnum
{
    [Description("Grátis")]
    [PlanTypeHelper.Price(0.00)]
    [PlanTypeHelper.SchedulingLimit(30)]
    [PlanTypeHelper.Description("Freelancers e autônomos")]
    [PlanTypeHelper.Perks(["28 agendamentos/mês", "Notificações por e-mail", "Sem suporte!"])]
    [PlanTypeHelper.DurationDays(SystemConsts.Time.PlanDurationDaysFree)]
    Free = 1,

    [Description("Básico")]
    [PlanTypeHelper.Price(9.99)]
    [PlanTypeHelper.SchedulingLimit(500)]
    [PlanTypeHelper.Description("Pequenas equipes")]
    [PlanTypeHelper.Perks(["Limite de agendamentos alto", "Integração com o WhatsApp", "Suporte prioritário"])]
    [PlanTypeHelper.DurationDays(SystemConsts.Time.PlanDurationDays)]
    Basic = 2,

    [Description("Premium")]
    [PlanTypeHelper.Price(19.99)]
    [PlanTypeHelper.SchedulingLimit(int.MaxValue)]
    [PlanTypeHelper.Description("Grandes operações")]
    [PlanTypeHelper.Perks(["Agendamentos ilimitados", "Integrações avançadas", "Suporte dedicado"])]
    [PlanTypeHelper.DurationDays(SystemConsts.Time.PlanDurationDays)]
    Premium = 3
}

/// <summary>
///  (decimal price, int schedulingLimit, string description, List<string> perks) = PlanTypeHelper.GetValues(PlanTypeEnum.Free);
/// </summary>
public class PlanTypeHelper()
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class PriceAttribute(double value) : Attribute
    {
        public decimal Value { get; } = (decimal)value;
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SchedulingLimitAttribute(int value) : Attribute
    {
        public int Value { get; } = value;
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DescriptionAttribute(string value) : Attribute
    {
        public string Value { get; } = value;
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class PerksAttribute(string[] values) : Attribute
    {
        public string[] Value { get; } = [.. values];
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DurationDaysAttribute(int value) : Attribute
    {
        public int Value { get; } = value;
    }

    /// <summary>
    /// Retorna o preço original do módulo sem aplicar desconto.
    /// Arredonda para 2 casas decimais usando MidpointRounding.AwayFromZero.
    /// Retorna 0 se o módulo não existir ou não tiver atributo de preço.
    /// Além disso, retorna o limite de agendamento, descrição, lista de propriedades (perks) e duração em dias. 
    /// </summary>
    public static (decimal price, int schedulingLimit, string description, string[] perks, int durationDays) GetValues(PlanTypeEnum module)
    {
        FieldInfo? field = typeof(PlanTypeEnum).GetField(module.ToString());

        if (field is null)
        {
            return (0m, 0, string.Empty, [], 0);
        }

        PriceAttribute? priceAttr = (PriceAttribute?)Attribute.GetCustomAttribute(field!, typeof(PriceAttribute));
        decimal price = priceAttr?.Value ?? 0m;
        decimal priceRound = decimal.Round(price, 2, MidpointRounding.AwayFromZero);

        SchedulingLimitAttribute? schedulingLimitAttr = (SchedulingLimitAttribute?)Attribute.GetCustomAttribute(field!, typeof(SchedulingLimitAttribute));
        int schedulingLimit = schedulingLimitAttr?.Value ?? 0;

        DescriptionAttribute? descriptionAttr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
        string description = descriptionAttr?.Value ?? string.Empty;

        PerksAttribute? perksAttr = (PerksAttribute?)Attribute.GetCustomAttribute(field!, typeof(PerksAttribute));
        string[] perks = perksAttr?.Value ?? [];

        DurationDaysAttribute? durationDaysAttr = (DurationDaysAttribute?)Attribute.GetCustomAttribute(field!, typeof(DurationDaysAttribute));
        int durationDays = durationDaysAttr?.Value ?? 0;

        return (priceRound, schedulingLimit, description, perks, durationDays);
    }
}
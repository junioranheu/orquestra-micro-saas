using System.ComponentModel;
using System.Reflection;

namespace Orquestra.Domain.Enums;

public enum PlanTypeEnum
{
    [Description("Grátis")]
    [PlanTypeHelper.Price(0.00)]
    [PlanTypeHelper.SchedulingLimit(30)]
    Free = 1,

    [Description("Básico")]
    [PlanTypeHelper.Price(9.99)]
    [PlanTypeHelper.SchedulingLimit(500)]
    Basic = 2,

    [Description("Premium")]
    [PlanTypeHelper.Price(19.99)]
    [PlanTypeHelper.SchedulingLimit(int.MaxValue)]
    Premium = 3
}

/// <summary>
/// decimal price1 = PlanTypeHelper.GetPrice(PlanTypeEnum.Free);
/// decimal price2 = PlanTypeHelper.GetPrice(PlanTypeEnum.Basic);  
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

    /// <summary>
    /// Retorna o preço original do módulo sem aplicar desconto.
    /// Arredonda para 2 casas decimais usando MidpointRounding.AwayFromZero.
    /// Retorna 0 se o módulo não existir ou não tiver atributo de preço.
    /// Além disso, retorna o limite de agendamento.
    /// </summary>
    public static (decimal price, int schedulingLimit) GetValues(PlanTypeEnum module)
    {
        FieldInfo? field = typeof(PlanTypeEnum).GetField(module.ToString());

        if (field is null)
        {
            return (0m, 0);
        }

        PriceAttribute? priceAttr = (PriceAttribute?)Attribute.GetCustomAttribute(field!, typeof(PriceAttribute));
        decimal price = priceAttr?.Value ?? 0m;
        decimal priceRound = decimal.Round(price, 2, MidpointRounding.AwayFromZero);

        SchedulingLimitAttribute? schedulingLimitAttr = (SchedulingLimitAttribute?)Attribute.GetCustomAttribute(field!, typeof(SchedulingLimitAttribute));
        int schedulingLimit = schedulingLimitAttr?.Value ?? 0;

        return (priceRound, schedulingLimit);
    }
}
using System.ComponentModel;
using System.Reflection;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Módulo de controle de agendamentos")]
    [ModuleHelper.Price(4.99)]
    Scheduling = 1,

    [Description("Módulo de controle financeiro")]
    [ModuleHelper.Price(7.99)]
    Sales = 2
}

/// <summary>
/// decimal price1 = ModuleHelper.GetPrice(ModuleEnum.Scheduling);
/// decimal price2 = ModuleHelper.GetPrice(ModuleEnum.Sales);  
/// </summary>
public class ModuleHelper()
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class PriceAttribute(double value) : Attribute
    {
        public decimal Value { get; } = (decimal)value;
    }

    public static decimal GetPrice(ModuleEnum module)
    {
        FieldInfo? field = typeof(ModuleEnum).GetField(module.ToString());
        PriceAttribute? attr = (PriceAttribute?)Attribute.GetCustomAttribute(field!, typeof(PriceAttribute));
        decimal price = attr?.Value ?? 0m;

        return price;
    }
}
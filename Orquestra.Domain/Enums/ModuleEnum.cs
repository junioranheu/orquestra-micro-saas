using System.ComponentModel;
using System.Reflection;

namespace Orquestra.Domain.Enums;

public enum ModuleEnum
{
    [Description("Módulo de controle de agendamentos")]
    [ModuleHelper.Price(7.99)]
    [ModuleHelper.Discount(37.55)]
    Scheduling = 1,

    [Description("Módulo de controle financeiro")]
    [ModuleHelper.Price(7.99)]
    [ModuleHelper.Discount(0)]
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

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DiscountAttribute(double percentage) : Attribute
    {
        public decimal Value { get; } = (decimal)percentage / 100m;
        public decimal Percentage { get; } = (decimal)percentage;
    }

    public static decimal GetOriginalPrice(ModuleEnum module)
    {
        FieldInfo? field = typeof(ModuleEnum).GetField(module.ToString());

        if (field is null) { 
            return 0m;
        }

        PriceAttribute? priceAttr = (PriceAttribute?)Attribute.GetCustomAttribute(field!, typeof(PriceAttribute));

        decimal price = priceAttr?.Value ?? 0m;
        decimal priceRound = decimal.Round(price, 2, MidpointRounding.AwayFromZero);

        return priceRound;
    }

    public static decimal GetDiscount(ModuleEnum module)
    {
        FieldInfo? field = typeof(ModuleEnum).GetField(module.ToString());

        if (field is null)
        {
            return 0m;
        }

        DiscountAttribute? discountAttr = (DiscountAttribute?)Attribute.GetCustomAttribute(field!, typeof(DiscountAttribute));

        decimal percentage = discountAttr?.Percentage ?? 0m;

        return percentage;
    }

    public static decimal GetPrice(ModuleEnum module)
    {
        decimal price = GetOriginalPrice(module);
        decimal discountPercent = GetDiscount(module);

        decimal finalPrice = price * (1 - (discountPercent / 100m));
        decimal finalPriceRound = decimal.Round(finalPrice, 2, MidpointRounding.AwayFromZero);

        return finalPriceRound;
    }
}
using Orquestra.Domain.Enums;
using System.Reflection;

namespace Orquestra.UnitTests.Tests.Enums;

public sealed class ModuleEnumTests
{
    [Fact]
    public void GetPrice_ShouldReturnValuesFromEnumAttributes()
    {
        ModuleEnum[] modules = Enum.GetValues<ModuleEnum>();

        foreach (var module in modules)
        {
            // Act;
            decimal price = ModuleHelper.GetOriginalPrice(module);
            decimal discount = ModuleHelper.GetDiscount(module);
            decimal finalPrice = ModuleHelper.GetPrice(module);

            // Obter os atributos reais do enum via reflection;
            FieldInfo field = typeof(ModuleEnum).GetField(module.ToString())!;

            ModuleHelper.PriceAttribute priceAttr = field.GetCustomAttribute<ModuleHelper.PriceAttribute>()!;
            ModuleHelper.DiscountAttribute discountAttr = field.GetCustomAttribute<ModuleHelper.DiscountAttribute>()!;

            decimal expectedPrice = decimal.Round(priceAttr.Value, 2, MidpointRounding.AwayFromZero);
            decimal expectedDiscount = discountAttr.Percentage;
            decimal expectedFinal = decimal.Round(expectedPrice * (1 - expectedDiscount / 100m), 2, MidpointRounding.AwayFromZero);

            // Assert;
            Assert.Equal(expectedPrice, price);
            Assert.Equal(expectedDiscount, discount);
            Assert.Equal(expectedFinal, finalPrice);
        }
    }

    [Fact]
    public void GetDiscount_ShouldReturnZero_WhenModuleDoesNotExist()
    {
        // Arrange;
        ModuleEnum sut = (ModuleEnum)999; // Módulo inexistente;

        // Act;
        decimal discount = ModuleHelper.GetDiscount(sut);

        // Assert;
        Assert.Equal(0m, discount);
    }

    [Fact]
    public void GetOriginalPrice_ShouldReturnZero_WhenModuleDoesNotExist()
    {
        // Arrange;
        ModuleEnum sut = (ModuleEnum)999; // Módulo inexistente;

        // Act;
        decimal price = ModuleHelper.GetOriginalPrice(sut);

        // Assert;
        Assert.Equal(0m, price);
    }
}
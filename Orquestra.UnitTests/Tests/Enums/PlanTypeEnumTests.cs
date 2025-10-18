using Orquestra.Domain.Enums;
using System.Reflection;

namespace Orquestra.UnitTests.Tests.Enums;

public sealed class PlanTypeEnumTests
{
    [Fact]
    public void GetPrice_ShouldReturnValuesFromEnumAttributes()
    {
        PlanTypeEnum[] modules = Enum.GetValues<PlanTypeEnum>();

        foreach (var module in modules)
        {
            // Act;
            (decimal price, int schedulingLimit, string _, string[] _, int _) = PlanTypeHelper.GetValues(module);

            // Obter os atributos reais do enum via reflection;
            FieldInfo field = typeof(PlanTypeEnum).GetField(module.ToString())!;

            PlanTypeHelper.PriceAttribute priceAttr = field.GetCustomAttribute<PlanTypeHelper.PriceAttribute>()!;
            PlanTypeHelper.SchedulingLimitAttribute schedulingLimitAttr = field.GetCustomAttribute<PlanTypeHelper.SchedulingLimitAttribute>()!;

            decimal expectedPrice = decimal.Round(priceAttr.Value, 2, MidpointRounding.AwayFromZero);
            int expectedSchedulingLimit = schedulingLimitAttr.Value;

            // Assert;
            Assert.Equal(expectedPrice, price);
            Assert.Equal(expectedSchedulingLimit, schedulingLimit);
        }
    }

    [Fact]
    public void GetOriginalPrice_ShouldReturnZero_WhenModuleDoesNotExist()
    {
        // Arrange;
        PlanTypeEnum module = (PlanTypeEnum)999; // Módulo inexistente;

        // Act;
        (decimal price, int _, string _, string[] _, int _) = PlanTypeHelper.GetValues(module);

        // Assert;
        Assert.Equal(0m, price);
    }
}
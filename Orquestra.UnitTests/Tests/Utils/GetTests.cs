using Orquestra.Utils.Fixtures;
using System.ComponentModel;

namespace Orquestra.UnitTests.Tests.Utils;

public sealed class GetTests
{
    [Fact]
    public void GetDate_ShouldReturn_UtcNowOrClose()
    {
        // Arrange
        DateTime before = DateTime.UtcNow;

        // Act
        DateTime result = Get.GetDate();

        // Assert
        DateTime after = DateTime.UtcNow;
        Assert.InRange(result, before, after);
    }

    [Fact]
    public void GetEnumDesc_ShouldReturn_EnumDescription()
    {
        // Act
        string desc = Get.GetEnumDesc(TestEnum.First);

        // Assert
        Assert.Equal("Primeiro Valor", desc);
    }

    [Fact]
    public void GetDateDetails_ShouldReturn_FormattedDate()
    {
        // Act
        string result = Get.GetDateDetails();

        // Assert
        Assert.Contains(" às ", result);
        Assert.Matches(@"\d{2}/\d{2}/\d{4} às \d{2}:\d{2}:\d{2}", result);
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(15, false)]
    public void GetRandomString_ShouldReturn_CorrectLength(int length, bool onlyUpper)
    {
        // Act
        string result = Get.GetRandomString(length, onlyUpper);

        // Assert
        Assert.Equal(length, result.Length);

        if (onlyUpper)
        {
            Assert.DoesNotContain(result, c => char.IsLower(c));
        }
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(50, 50)]
    public void GetRandomNumber_ShouldReturnWithinRange(int min, int max)
    {
        // Act
        int result = Get.GetRandomNumber(min, max);

        // Assert
        Assert.InRange(result, min, max);
    }

    [Theory]
    [InlineData("JuNioR ROBerto dE soUZA", "Junior Roberto de Souza")]
    [InlineData("MARIAna ScalzaRETTO", "Mariana Scalzaretto")]
    public void NormalizeToProperName_ShouldReturn_ProperCase(string input, string expected)
    {
        // Act
        string result = Get.NormalizeToProperName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateTrueOrFalse_ShouldReturn_Bool()
    {
        // Act
        bool result = Get.GenerateTrueOrFalse();

        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void GenerateTrueOrFalse_ShouldRespect_HitChance()
    {
        // Act
        bool alwaysTrue = Get.GenerateTrueOrFalse(100);
        bool alwaysFalse = Get.GenerateTrueOrFalse(0);

        // Assert
        Assert.True(alwaysTrue);
        Assert.False(alwaysFalse);
    }

    #region helpers
    private enum TestEnum
    {
        [Description("Primeiro Valor")]
        First,

        [Description("Segundo Valor")]
        Second
    }
    #endregion
}
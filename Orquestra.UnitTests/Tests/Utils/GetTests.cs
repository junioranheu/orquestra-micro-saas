using System.ComponentModel;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Tests.Utils;

public sealed class GetTests
{
    [Fact]
    public void GetDate_ShouldReturn_UtcNowOrClose()
    {
        // Arrange
        DateTime before = DateTime.UtcNow;

        // Act
        DateTime result = GetDate();

        // Assert
        DateTime after = DateTime.UtcNow;
        Assert.InRange(result, before, after);
    }

    [Fact]
    public void GetEnumDesc_ShouldReturn_EnumDescription()
    {
        // Act
        string desc = GetEnumDesc(TestEnum.First);

        // Assert
        Assert.Equal("Primeiro valor", desc);
    }

    [Fact]
    public void GetDateDetails_ShouldReturn_FormattedDate()
    {
        // Act
        string result = GetDateDetails();

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
        string result = GetRandomString(length, onlyUpper);

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
        int result = GetRandomNumber(min, max);

        // Assert
        Assert.InRange(result, min, max);
    }

    [Theory]
    [InlineData("JuNioR ROBerto dE soUZA", "Junior Roberto de Souza")]
    [InlineData("MARIAna ScalzaRETTO", "Mariana Scalzaretto")]
    public void NormalizeToProperName_ShouldReturn_ProperCase(string input, string expected)
    {
        // Act
        string result = NormalizeToProperName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateTrueOrFalse_ShouldReturn_Bool()
    {
        // Act
        bool result = GenerateTrueOrFalse();

        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void GenerateTrueOrFalse_ShouldRespect_HitChance()
    {
        // Act
        bool alwaysTrue = GenerateTrueOrFalse(100);
        bool alwaysFalse = GenerateTrueOrFalse(0);

        // Assert
        Assert.True(alwaysTrue);
        Assert.False(alwaysFalse);
    }

    [Fact]
    public void GetFirstPart_ShouldReturn_FirstWord_WhenStringHasMultipleWords()
    {
        // Arrange
        string input = "Junior de Souza";

        // Act
        string result = GetFirstWord(input);

        // Assert
        Assert.Equal("Junior", result);
    }

    [Fact]
    public void GetFirstPart_ShouldReturn_WholeString_WhenNoDelimiterFound()
    {
        // Arrange
        string input = "Orquestra";

        // Act
        string result = GetFirstWord(input);

        // Assert
        Assert.Equal("Orquestra", result);
    }

    [Fact]
    public void GetFirstPart_ShouldReturn_Empty_WhenInputIsNullOrEmpty()
    {
        // Arrange
        string? nullInput = null;
        string emptyInput = "";

        // Act
        string resultNull = GetFirstWord(nullInput);
        string resultEmpty = GetFirstWord(emptyInput);

        // Assert
        Assert.Equal(string.Empty, resultNull);
        Assert.Equal(string.Empty, resultEmpty);
    }

    [Fact]
    public void GetFirstPart_ShouldRespect_CustomDelimiter()
    {
        // Arrange
        string input = "part1|part2|part3";

        // Act
        string result = GetFirstWord(input, '|');

        // Assert
        Assert.Equal("part1", result);
    }

    #region helpers
    private enum TestEnum
    {
        [Description("Primeiro valor")]
        First,

        [Description("Segundo valor")]
        Second
    }
    #endregion
}
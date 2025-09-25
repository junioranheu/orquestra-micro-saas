using Orquestra.Domain.Consts;
using System.ComponentModel;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Tests.Utils;

public sealed class GetTests
{
    [Fact]
    public void GetDate_ShouldReturn_UtcNowOrClose()
    {
        // Arrange;
        DateTime before = DateTime.UtcNow;

        // Act;
        DateTime result = GetDate();

        // Assert;
        DateTime after = DateTime.UtcNow;
        Assert.InRange(result, before, after);
    }

    [Fact]
    public void GetEnumDesc_ShouldReturn_EnumDescription()
    {
        // Act;
        string desc = GetEnumDesc(TestEnum.First);

        // Assert;
        Assert.Equal("Primeiro valor", desc);
    }

    [Fact]
    public void GetDateDetails_ShouldReturn_FormattedDate()
    {
        // Act;
        string result = GetDateDetails();

        // Assert;
        Assert.Contains(" às ", result);
        Assert.Matches(@"\d{2}/\d{2}/\d{4} às \d{2}:\d{2}:\d{2}", result);
    }

    [Fact]
    public void GetDateDetails_ShouldReturn_FormattedDate_WithoutHour()
    {
        // Act;
        string result = GetDateDetails(withHour: false);

        // Assert;
        Assert.Matches(@"\d{2}/\d{2}/\d{4}", result);
    }

    [Theory]
    [InlineData(10, true)]
    [InlineData(15, false)]
    public void GetRandomString_ShouldReturn_CorrectLength(int length, bool onlyUpper)
    {
        // Act;
        string result = GetRandomString(length, onlyUpper);

        // Assert;
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
        // Act;
        int result = GetRandomNumber(min, max);

        // Assert;
        Assert.InRange(result, min, max);
    }

    [Theory]
    [InlineData("JuNioR ROBerto dE soUZA", "Junior Roberto de Souza")]
    [InlineData("MARIAna ScalzaRETTO", "Mariana Scalzaretto")]
    public void NormalizeToProperName_ShouldReturn_ProperCase(string input, string expected)
    {
        // Act;
        string result = NormalizeToProperName(input);

        // Assert;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateTrueOrFalse_ShouldReturn_Bool()
    {
        // Act;
        bool result = GenerateTrueOrFalse();

        // Assert;
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void GenerateTrueOrFalse_ShouldRespect_HitChance()
    {
        // Act;
        bool alwaysTrue = GenerateTrueOrFalse(100);
        bool alwaysFalse = GenerateTrueOrFalse(0);

        // Assert;
        Assert.True(alwaysTrue);
        Assert.False(alwaysFalse);
    }

    [Fact]
    public void GetFirstPart_ShouldReturn_FirstWord_WhenStringHasMultipleWords()
    {
        // Arrange;
        string input = "Junior de Souza";

        // Act;
        string result = GetFirstWord(input);

        // Assert;
        Assert.Equal("Junior", result);
    }

    [Fact]
    public void GetFirstPart_ShouldReturn_WholeString_WhenNoDelimiterFound()
    {
        // Arrange;
        string input = SystemConsts.NameApp;

        // Act;
        string result = GetFirstWord(input);

        // Assert;
        Assert.Equal(SystemConsts.NameApp, result);
    }

    [Fact]
    public void GetFirstPart_ShouldReturn_Empty_WhenInputIsNullOrEmpty()
    {
        // Arrange;
        string? nullInput = null;
        string emptyInput = "";

        // Act;
        string resultNull = GetFirstWord(nullInput);
        string resultEmpty = GetFirstWord(emptyInput);

        // Assert;
        Assert.Equal(string.Empty, resultNull);
        Assert.Equal(string.Empty, resultEmpty);
    }

    [Fact]
    public void GetFirstPart_ShouldRespect_CustomDelimiter()
    {
        // Arrange;
        string input = "part1|part2|part3";

        // Act;
        string result = GetFirstWord(input, '|');

        // Assert;
        Assert.Equal("part1", result);
    }

    [Fact]
    public void GenerateSafeToken32_ShouldReturn_NonNullBase64String()
    {
        // Act;
        string token = GenerateSafeToken32Bytes(urlSafe: false);

        // Assert;
        Assert.False(string.IsNullOrWhiteSpace(token));

        // Base64 de 32 bytes gera geralmente 44 caracteres;
        Assert.Equal(44, token.Length);

        // Verifica se é Base64 válido;
        Span<byte> buffer = new(new byte[32]);
        bool isBase64 = Convert.TryFromBase64String(token, buffer, out _);
        Assert.True(isBase64);
    }

    [Fact]
    public void GenerateSafeToken32_UrlSafe_ShouldReturn_NonNullStringWithoutIllegalChars()
    {
        // Act;
        string token = GenerateSafeToken32Bytes(urlSafe: true);

        // Assert;
        Assert.False(string.IsNullOrWhiteSpace(token));

        // Não deve conter caracteres problemáticos para URL;
        Assert.DoesNotContain('+', token);
        Assert.DoesNotContain('/', token);
        Assert.DoesNotContain('=', token);
    }

    [Fact]
    public void GenerateSafeToken32_ShouldGenerate_UniqueTokens()
    {
        // Act;
        string token1 = GenerateSafeToken32Bytes(urlSafe: true);
        string token2 = GenerateSafeToken32Bytes(urlSafe: true);

        // Assert;
        Assert.NotEqual(token1, token2);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData(" Hello ", "hello")]
    [InlineData("HELLO", "hello")]
    [InlineData(" HeLLo WoRLd ", "hello world")]
    public void GetNormalizedLowerStr_ShouldReturn_ExpectedResult(string? input, string expected)
    {
        // Act;
        var result = GetNormalizedLowerStr(input);

        // Assert;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("teste@email.com", true)]
    [InlineData("teste+alias@email.com", true)]
    [InlineData("TESTE@EMAIL.COM", true)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    [InlineData("email_invalido", false)]
    [InlineData("email@invalido", false)]
    [InlineData("email@invalido.", false)]
    [InlineData("@semusuario.com", false)]
    public void IsEmailValid_ShouldReturn_ExpectedResult(string? email, bool expected)
    {
        // Act;
        bool result = IsEmailValid(email);

        // Assert;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateJwtSecret_ShouldReturn_32CharHexString()
    {
        // Act;
        string secret = GenerateJwtSecret();

        // Assert;
        Assert.NotNull(secret);
        Assert.Equal(32, secret.Length); // Tem 32 caracteres;
        Assert.True(secret.All(x => "0123456789abcdef".Contains(char.ToLower(x)))); // Todos hex;
    }

    [Fact]
    public void GuidToNumericId_ShouldReturnPositiveNumber()
    {
        // Arrange;
        Guid guid = Guid.NewGuid();

        // Act;
        long result = GuidToNumericId(guid);

        // Assert;
        Assert.True(result > 0, "O número gerado deve ser positivo.");
    }

    [Fact]
    public void GuidToNumericId_ShouldGenerateSameResult_ForSameGuid()
    {
        // Arrange;
        Guid guid = Guid.NewGuid();

        // Act;
        long result1 = GuidToNumericId(guid);
        long result2 = GuidToNumericId(guid);

        // Assert;
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void GuidToNumericId_ShouldGenerateDifferentResults_ForDifferentGuids()
    {
        // Arrange;
        Guid guid1 = Guid.NewGuid();
        Guid guid2 = Guid.NewGuid();

        // Act;
        long result1 = GuidToNumericId(guid1);
        long result2 = GuidToNumericId(guid2);

        // Assert;
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void GetDatesDiffInMinutes_ShouldReturnZero_WhenDatesAreEqual()
    {
        // Arrange;
        DateTime start = new(2025, 09, 25, 10, 00, 00);
        DateTime end = new(2025, 09, 25, 10, 00, 00);

        // Act;
        int result = GetDatesDiffInMinutes(start, end);

        // Assert;
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetDatesDiffInMinutes_ShouldReturnPositive_WhenEndIsAfterStart()
    {
        // Arrange;
        DateTime start = new(2025, 09, 25, 10, 00, 00);
        DateTime end = new(2025, 09, 25, 10, 30, 00);

        // Act;
        int result = GetDatesDiffInMinutes(start, end);

        // Assert;
        Assert.Equal(30, result);
    }

    [Fact]
    public void GetDatesDiffInMinutes_ShouldReturnNegative_WhenEndIsBeforeStart()
    {
        // Arrange;
        DateTime start = new(2025, 09, 25, 10, 30, 00);
        DateTime end = new(2025, 09, 25, 10, 00, 00);

        // Act;
        int result = GetDatesDiffInMinutes(start, end);

        // Assert;
        Assert.Equal(-30, result);
    }

    [Fact]
    public void GetDatesDiffInMinutes_ShouldIgnoreSeconds()
    {
        // Arrange;
        DateTime start = new(2025, 09, 25, 10, 00, 00);
        DateTime end = new(2025, 09, 25, 10, 00, 59);

        // Act;
        int result = GetDatesDiffInMinutes(start, end);

        // Assert;
        Assert.Equal(0, result); // 59 segundos < 1 minuto;
    }

    [Fact]
    public void GetDatesDiffInMinutes_ShouldTruncateDecimalPart()
    {
        // Arrange;
        DateTime start = new(2025, 09, 25, 10, 00, 00);
        DateTime end = new(2025, 09, 25, 10, 01, 59);

        // Act;
        int result = GetDatesDiffInMinutes(start, end);

        // Assert;
        Assert.Equal(1, result); // Truncou o 1,98 min;
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
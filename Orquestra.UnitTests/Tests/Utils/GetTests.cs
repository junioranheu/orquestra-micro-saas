using Microsoft.AspNetCore.Http;
using Orquestra.Domain.Consts;
using System.ComponentModel;
using System.Text.Json;
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
    public void ConvertToUtc_WhenDateIsUtc_ReturnsSameDate()
    {
        // Arrange;
        DateTime utcDate = new(2025, 9, 29, 21, 0, 0, DateTimeKind.Utc);

        // Act;
        DateTime result = ConvertToUtc(utcDate);

        // Assert;
        Assert.Equal(utcDate, result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void ConvertToUtc_WhenDateIsLocal_ConvertsToUtc()
    {
        // Arrange;
        DateTime localDate = new(2025, 9, 29, 21, 0, 0, DateTimeKind.Local);

        // Act;
        DateTime result = ConvertToUtc(localDate);

        // Assert;
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(localDate.ToUniversalTime(), result);
    }

    [Fact]
    public void ConvertToUtc_WhenDateIsUnspecified_AssumesLocalAndConverts()
    {
        // Arrange;
        DateTime unspecifiedDate = new(2025, 9, 29, 21, 0, 0, DateTimeKind.Unspecified);

        // Act;
        DateTime result = ConvertToUtc(unspecifiedDate);

        // Assert;
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Local).ToUniversalTime(), result);
    }

    [Fact]
    public void ConvertToBrasiliaTime_WithUtcDate_ReturnsCorrectBrasiliaTime()
    {
        // Arrange;
        DateTime utcDate = new(2025, 9, 26, 12, 0, 0, DateTimeKind.Utc);

        // Act;
        DateTime result = ConvertToBrasiliaTime(utcDate);

        // Assert;
        Assert.Equal(utcDate.AddHours(-3), result); // Brasília normalmente UTC-3;
    }

    [Fact]
    public void ConvertToBrasiliaTime_WithLocalDate_ReturnsCorrectBrasiliaTime()
    {
        // Arrange;
        DateTime localDate = new(2025, 9, 26, 12, 0, 0, DateTimeKind.Local);

        // Act;
        var result = ConvertToBrasiliaTime(localDate);

        // Assert;
        DateTime expected = localDate.ToUniversalTime().AddHours(-3);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertToBrasiliaTime_WithUnspecifiedDate_AssumesLocalAndReturnsCorrectBrasiliaTime()
    {
        // Arrange;
        DateTime unspecifiedDate = new(2025, 9, 26, 12, 0, 0, DateTimeKind.Unspecified);

        // Act;
        var result = ConvertToBrasiliaTime(unspecifiedDate);

        // Assert;
        DateTime expected = DateTime.SpecifyKind(unspecifiedDate, DateTimeKind.Local).ToUniversalTime().AddHours(-3);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertToBrasiliaTime_InvalidTimeZone_ThrowsException()
    {
        // Simulação: Alterar ID de timezone para teste de exceção;
        DateTime invalidDate = new(2025, 9, 26, 12, 0, 0, DateTimeKind.Utc);

        // Act & Assert;
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            TimeZoneInfo.FindSystemTimeZoneById("InvalidTimeZone");
        });
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
    public void GetEnumDesc_ShouldReturn_EnumWithoutDescription()
    {
        // Act;
        string desc = GetEnumDesc(TestEnum.Third);

        // Assert;
        Assert.Equal("Third", desc);
    }

    [Fact]
    public void GetDateDetails_ShouldReturn_FormattedDate_UsingExistentDate()
    {
        // Act;
        string result = GetDateDetails(date: GetDate());

        // Assert;
        Assert.Contains(" às ", result);
        Assert.Matches(@"\d{2}/\d{2}/\d{4} às \d{2}:\d{2}:\d{2}", result);
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
    [InlineData(10, true, true)] // Só maiúsculas;
    [InlineData(12, false, true)] // Maiúsculas + minúsculas;
    [InlineData(15, true, false)] // Maiúsculas + números;
    [InlineData(20, false, false)] // Maiúsculas + minúsculas + números;
    public void GetRandomString_ShouldReturn_CorrectLength_AndRespectFlags(int length, bool onlyUpper, bool onlyLetters)
    {
        // Act;
        string result = GetRandomString(length, onlyUpper, onlyLetters);

        // Assert;
        Assert.Equal(length, result.Length);

        if (onlyUpper && onlyLetters)
        {
            // Apenas maiúsculas;
            Assert.All(result, c => Assert.True(char.IsUpper(c)));
        }
        else if (!onlyUpper && onlyLetters)
        {
            // Apenas letras (maiúsculas + minúsculas);
            Assert.All(result, c => Assert.True(char.IsLetter(c)));
        }
        else if (onlyUpper && !onlyLetters)
        {
            // Maiúsculas + números;
            Assert.All(result, c => Assert.True(char.IsUpper(c) || char.IsDigit(c)));
        }
        else
        {
            // Maiúsculas + minúsculas + números;
            Assert.All(result, c => Assert.True(char.IsLetterOrDigit(c)));
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
        string input = SystemConsts.App.NameApp;

        // Act;
        string result = GetFirstWord(input);

        // Assert;
        Assert.Equal(SystemConsts.App.NameApp, result);
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

    [Fact]
    public void GetCountries_ShouldNotReturnEmpty()
    {
        // Act;
        List<string> countries = GetCountries();

        // Assert;
        Assert.NotEmpty(countries);
    }

    [Fact]
    public void GetCountries_ShouldContainBrazil()
    {
        // Act;
        List<string> countries = GetCountries();

        // Assert;
        Assert.Contains("Brasil", countries);
    }

    [Fact]
    public void GetCountries_ShouldBeAlphabeticallyOrdered()
    {
        // Act;
        List<string> countries = GetCountries();

        // Assert;
        List<string> ordered = countries.OrderBy(x => x).ToList();
        Assert.Equal(ordered, countries);
    }

    [Fact]
    public void GetCountries_ShouldNotHaveDuplicates()
    {
        // Act;
        List<string> countries = GetCountries();

        // Assert;
        Assert.Equal(countries.Count, countries.Distinct().Count());
    }

    [Theory]
    [InlineData("Brasil", "brasil")]
    [InlineData("BRASIL", "brasil")]
    [InlineData("brásil", "brasil")]
    [InlineData(" BRÁSil ", "brasil")]
    [InlineData("Árvore", "arvore")]
    [InlineData("Êxemplo", "exemplo")]
    [InlineData("Çapim", "capim")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    public void NormalizeTextRemoveAccentsAndLower_ShouldNormalizeCorrectly(string? input, string expected)
    {
        // Act;
        string result = NormalizeTextRemoveAccentsAndLower(input);

        // Assert;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Should_NotThrow_When_SizeIsWithinLimit()
    {
        // Arrange;
        var bytes = new byte[512 * 1024]; // 0.5 MB;
        Action CreateSut() => () => ValidateMaxSizeBytes(bytes, 1);

        // Act;
        Action act = CreateSut();

        // Assert;
        act(); // Não deve lançar exceção;
    }

    [Fact]
    public void Should_ThrowInvalidOperation_When_SizeExceedsLimit()
    {
        // Arrange;
        var bytes = new byte[2 * 1024 * 1024]; // 2 MB;
        Action CreateSut() => () => ValidateMaxSizeBytes(bytes, 1);

        // Act & assert;
        Assert.Throws<InvalidOperationException>(CreateSut());
    }

    [Fact]
    public void Should_ThrowArgumentNull_When_InputIsNull()
    {
        // Arrange;
        byte[]? bytes = null;
        Action CreateSut() => () => ValidateMaxSizeBytes(bytes, 1);

        // Act & assert;
        Assert.Throws<ArgumentNullException>(CreateSut());
    }

    [Fact]
    public void ValidateMaxSizeFile_FileIsNull_ThrowsArgumentNullException()
    {
        IFormFile? file = null;

        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => ValidateMaxSizeFile(file, 1));

        Assert.Equal("file", ex.ParamName);
    }

    [Fact]
    public void ValidateMaxSizeFile_FileExceedsLimit_ThrowsInvalidOperationException()
    {
        // Criar arquivo fake maior que o limite;
        byte[] fileContent = new byte[6 * 1024 * 1024]; // 6MB;
        IFormFile file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "Data", "test.png");

        Assert.Throws<InvalidOperationException>(() => ValidateMaxSizeFile(file, 5)); // Limite: 5MB;
    }

    [Fact]
    public void ValidateMaxSizeFile_FileWithinLimit_DoesNotThrow()
    {
        byte[] fileContent = new byte[2 * 1024 * 1024]; // 2MB;
        IFormFile file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, "Data", "test.png");

        Exception? ex = Record.Exception(() => ValidateMaxSizeFile(file, 5)); // Limite: 5MB;

        Assert.Null(ex); // Não deve lançar exceção;
    }

    [Fact]
    public void Should_Convert_Bytes_To_Base64_With_Valid_ContentType()
    {
        // Arrange;
        byte[] bytes = [1, 2, 3];
        string contentType = "image/png";

        // Act;
        string result = ConvertBytesToBase64(bytes, contentType);

        // Assert;
        Assert.StartsWith("data:image/png;base64,", result);
        Assert.Contains(Convert.ToBase64String(bytes), result);
    }

    [Fact]
    public void Should_Use_OctetStream_When_ContentType_Is_Null()
    {
        // Arrange;
        byte[] bytes = [1, 2, 3];

        // Act;
        string result = ConvertBytesToBase64(bytes, null);

        // Assert;
        Assert.StartsWith("data:application/octet-stream;base64,", result);
        Assert.Contains(Convert.ToBase64String(bytes), result);
    }

    [Fact]
    public void Should_Use_OctetStream_When_ContentType_Is_Empty()
    {
        // Arrange;
        byte[] bytes = [4, 5, 6];

        // Act;
        string result = ConvertBytesToBase64(bytes, "");

        // Assert;
        Assert.StartsWith("data:application/octet-stream;base64,", result);
        Assert.Contains(Convert.ToBase64String(bytes), result);
    }

    [Fact]
    public void Should_Throw_Exception_When_Bytes_Are_Null()
    {
        // Act & Assert;
        Assert.Throws<ArgumentException>(() => ConvertBytesToBase64(null!, "image/png"));
    }

    [Fact]
    public void Should_Throw_Exception_When_Bytes_Are_Empty()
    {
        // Arrange;
        byte[] empty = [];

        // Act & Assert;
        Assert.Throws<ArgumentException>(() => ConvertBytesToBase64(empty, "image/png"));
    }

    [Fact]
    public void Should_Return_All_Enum_Values_With_Descriptions_When_Available()
    {
        // Act;
        var result = GetEnumListWithDescriptions<TestEnumWithDescription>();

        // Assert;
        Assert.Equal(3, result.Count);

        Assert.Contains(result, x => x.Value == TestEnumWithDescription.First && x.Description == "First value");
        Assert.Contains(result, x => x.Value == TestEnumWithDescription.Second && x.Description == "Second value");
        Assert.Contains(result, x => x.Value == TestEnumWithDescription.NoDescription && x.Description == "NoDescription");
    }

    [Fact]
    public void Should_Use_Enum_Name_As_Description_When_No_DescriptionAttribute_Is_Present()
    {
        // Act;
        var result = GetEnumListWithDescriptions<TestEnumWithoutDescription>();

        // Assert;
        Assert.Equal(3, result.Count);

        foreach (var (_, name, description) in result)
        {
            Assert.Equal(name, description);
        }
    }

    [Fact]
    public void Should_Maintain_Enum_Order()
    {
        // Act;
        var result = GetEnumListWithDescriptions<TestEnumWithDescription>();

        // Assert;
        Assert.Equal(TestEnumWithDescription.First, result[0].Value);
        Assert.Equal(TestEnumWithDescription.Second, result[1].Value);
        Assert.Equal(TestEnumWithDescription.NoDescription, result[2].Value);
    }

    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    public void Should_Return_True_For_Valid_CPFs(string cpf)
    {
        // Act;
        bool result = IsValidCPF(cpf);

        // Assert;
        Assert.True(result);
    }

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("52998224724")]
    public void Should_Return_False_For_Invalid_CPFs(string cpf)
    {
        // Act;
        bool result = IsValidCPF(cpf);

        // Assert;
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("529.982.247")]
    public void Should_Return_False_For_Null_Or_Invalid_Length(string? cpf)
    {
        // Act;
        bool result = IsValidCPF(cpf);

        // Assert;
        Assert.False(result);
    }

    [Fact]
    public void Should_Remove_NonNumeric_Characters_Before_Validation()
    {
        // Arrange;
        string formatted = "529.982.247-25";
        string unformatted = "52998224725";

        // Act;
        bool result1 = IsValidCPF(formatted);
        bool result2 = IsValidCPF(unformatted);

        // Assert;
        Assert.True(result1);
        Assert.True(result2);
    }

    [Theory]
    [InlineData("<b>Olá mundo</b>", "Olá mundo")]
    [InlineData("<div>Teste</div>", "Teste")]
    [InlineData("<p>Com <span>tags</span> dentro</p>", "Com tags dentro")]
    [InlineData("Sem tags", "Sem tags")]
    [InlineData("", "")]
    [InlineData(null, "")]
    [InlineData("  <i>Espaços antes e depois</i>  ", "Espaços antes e depois")]
    public void RemoveHtmlTags_ShouldReturnCleanText(string? input, string expected)
    {
        // Act;
        string result = RemoveHtmlTags(input);

        // Assert;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{\"UserId\":\"b3461127-72cd-0585-eb71-adb3a38e8387\"}", "UserId", "b3461127-72cd-0585-eb71-adb3a38e8387")]
    [InlineData("{\"Age\":25}", "Age", "25")]
    [InlineData("{\"Name\":\"Junior\"}", "Name", "Junior")]
    [InlineData("{\"Active\":true}", "Active", "True")]
    [InlineData(null, "UserId", null)]
    [InlineData("", "UserId", null)]
    [InlineData("{}", "UserId", null)]
    [InlineData("{invalid json}", "UserId", null)]
    public void GetPropertyValue_ShouldReturnCorrectValue(string? json, string propertyName, string? expected)
    {
        // Act;
        if (typeof(Guid) == typeof(Guid) && expected != null && Guid.TryParse(expected, out Guid guidExpected))
        {
            Guid result = GetPropertyValueFromStringJson<Guid>(json, propertyName);
            Assert.Equal(guidExpected, result);
        }
        else if (typeof(bool) == typeof(bool) && bool.TryParse(expected, out bool boolExpected))
        {
            bool result = GetPropertyValueFromStringJson<bool>(json, propertyName);
            Assert.Equal(boolExpected, result);
        }
        else if (typeof(int) == typeof(int) && int.TryParse(expected, out int intExpected))
        {
            int result = GetPropertyValueFromStringJson<int>(json, propertyName);
            Assert.Equal(intExpected, result);
        }
        else
        {
            string? result = GetPropertyValueFromStringJson<string>(json, propertyName);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void Should_ReturnTrue_When_RunningFromXUnit()
    {
        // Act;
        bool result = IsRunningFromXUnit();

        // Assert;
        Assert.True(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeBrazilianPhone_ShouldReturnNull_WhenInputIsEmptyOrNull(string? input)
    {
        string? result = NormalizeBrazilianPhone(input);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("+55 (12) 98271-6339", "+5512982716339")]
    [InlineData("55 12 98271-6339", "+5512982716339")]
    [InlineData("(12) 98271-6339", "+5512982716339")]
    [InlineData("12 98271-6339", "+5512982716339")]
    [InlineData("12982716339", "+5512982716339")] // 11 dígitos;
    [InlineData("5512982716339", "+5512982716339")] // Já está com DDI + DDD;
    [InlineData("+5512982716339", "+5512982716339")] // Já está certo;
    [InlineData("5511987654321", "+5511987654321")] // Outro DDD, válido;
    public void NormalizeBrazilianPhone_ShouldNormalizeCorrectly(string input, string expected)
    {
        string? result = NormalizeBrazilianPhone(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("982716339")]
    [InlineData("123456")]
    [InlineData("987654321012345")]
    [InlineData("abc")]
    [InlineData("+999999999999999")]
    [InlineData("00982716339")]
    public void NormalizeBrazilianPhone_ShouldReturnNull_WhenInvalid(string input)
    {
        string? result = NormalizeBrazilianPhone(input);
        Assert.Null(result);
    }

    #region helpers
    private enum TestEnum
    {
        [Description("Primeiro valor")]
        First,

        [Description("Segundo valor")]
        Second,

        Third
    }

    private enum TestEnumWithDescription
    {
        [Description("First value")]
        First,

        [Description("Second value")]
        Second,

        NoDescription
    }

    private enum TestEnumWithoutDescription
    {
        One,
        Two,
        Three
    }
    #endregion
}
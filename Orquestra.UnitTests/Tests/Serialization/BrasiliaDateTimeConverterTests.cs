using Orquestra.Infrastructure.Serialization;
using System.Text;
using System.Text.Json;

namespace Orquestra.UnitTests.Tests.Serialization;

public class BrasiliaDateTimeConverterTests
{
    private readonly JsonSerializerOptions _options;
    private readonly BrasiliaDateTimeConverter _converter;

    public BrasiliaDateTimeConverterTests()
    {
        _options = new JsonSerializerOptions { Converters = { new BrasiliaDateTimeConverter() } };
        _converter = new BrasiliaDateTimeConverter();
    }

    [Fact]
    public void Write_ShouldConvertToBrasiliaTime()
    {
        // Arrange;
        DateTime utcTime = new(2025, 08, 21, 12, 0, 0, DateTimeKind.Utc);

        // Act;
        string json = JsonSerializer.Serialize(utcTime, _options);

        // Assert;
        // Hora de Brasília = UTC-3;
        Assert.Equal("\"2025-08-21T09:00:00\"", json);
    }

    [Fact]
    public void Write_ShouldFormatCorrectly()
    {
        // Arrange;
        DateTime localTime = new(2025, 08, 21, 15, 30, 45);

        // Act;
        string json = JsonSerializer.Serialize(localTime, _options);

        // Assert;
        // Só verifica o formato yyyy-MM-ddTHH:mm:ss;
        Assert.Matches(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}", json.Trim('"'));
    }

    [Fact]
    public void Read_ShouldParseValidDateTimeString()
    {
        // Arrange;
        string jsonDate = "\"2025-08-21T15:30:45\"";
        Utf8JsonReader reader = new(Encoding.UTF8.GetBytes(jsonDate));
        reader.Read();

        // Act;
        DateTime result = _converter.Read(ref reader, typeof(DateTime), new JsonSerializerOptions());

        // Assert;
        Assert.Equal(new DateTime(2025, 8, 21, 15, 30, 45), result);
    }

    [Fact]
    public void Read_ShouldReturnDefault_OnEmptyString()
    {
        // Arrange;
        string jsonDate = "\"\"";
        Utf8JsonReader reader = new(Encoding.UTF8.GetBytes(jsonDate));
        reader.Read();

        // Act;
        DateTime result = _converter.Read(ref reader, typeof(DateTime), new JsonSerializerOptions());

        // Assert;
        Assert.Equal(default, result);
    }
}
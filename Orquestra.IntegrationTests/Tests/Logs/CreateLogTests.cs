using Microsoft.Extensions.Logging;
using Moq;
using Orquestra.Application.UseCases.Logs.Create;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;

namespace Orquestra.IntegrationTests.Tests.Logs;

public sealed class CreateLogTests
{
    [Fact]
    public async Task Execute_ShouldSaveLog_WhenValidInput()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        CreateLog sut = CreateSut(context);

        Log log = new()
        {
            LogId = Guid.NewGuid(),
            RequestType = "GET",
            Endpoint = "/api/test",
            Parameters = "{ id: 1 }",
            Description = "Teste de log",
            Status = 200,
            UserId = Guid.NewGuid(),
            Exception = string.Empty
        };

        // Act;
        await sut.Execute(log);

        // Assert;
        Log? saved = await context.Logs.FindAsync(log.LogId);

        Assert.NotNull(saved);
        Assert.Equal("GET", saved.RequestType);
        Assert.Equal("/api/test", saved.Endpoint);
        Assert.Equal("Teste de log", saved.Description);
        Assert.Equal(200, saved.Status);
    }

    #region helpers
    private static CreateLog CreateSut(Context context)
    {
        Mock<ILogger<CreateLog>> loggerMock = new();
        CreateLog createLog = new(context, loggerMock.Object);

        return createLog;
    }
    #endregion
}
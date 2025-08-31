using Orquestra.Application.UseCases.Logs.Get;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Logs;

public sealed class GetLogTests
{
    [Fact]
    public async Task Execute_ShouldReturnAllLogs_WhenUserIdIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user1 = UserMock.Create("User One", "one@test.com", UserRoleEnum.Administrator);
        User user2 = UserMock.Create("User Two", "two@test.com", UserRoleEnum.Common);

        await Fixture.Save(context, user1);
        await Fixture.Save(context, user2);

        await Fixture.Save(context, new Log { LogId = Guid.NewGuid(), UserId = user1.UserId, Description = "Log 1 U1", Status = 200 });
        await Fixture.Save(context, new Log { LogId = Guid.NewGuid(), UserId = user1.UserId, Description = "Log 2 U1", Status = 200 });
        await Fixture.Save(context, new Log { LogId = Guid.NewGuid(), UserId = user2.UserId, Description = "Log 3 U2", Status = 500 });

        GetLog sut = CreateSut(context);

        PaginationInput pagination = new () { Index = 0, Limit = 10 };

        // Act;
        (IEnumerable<Log> output, int count) = await sut.Execute(pagination, null);

        // Assert;
        Assert.Equal(3, count);
        Assert.Equal(3, output.Count());
    }

    [Fact]
    public async Task Execute_ShouldReturnLogsFilteredByUserId()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user1 = UserMock.Create("User One", "one@test.com", UserRoleEnum.Administrator);
        User user2 = UserMock.Create("User Two", "two@test.com", UserRoleEnum.Common);

        await Fixture.Save(context, user1);
        await Fixture.Save(context, user2);

        await Fixture.Save(context, new Log { LogId = Guid.NewGuid(), UserId = user1.UserId, Description = "Log 1 U1", Status = 200 });
        await Fixture.Save(context, new Log { LogId = Guid.NewGuid(), UserId = user1.UserId, Description = "Log 2 U1", Status = 200 });
        await Fixture.Save(context, new Log { LogId = Guid.NewGuid(), UserId = user2.UserId, Description = "Log 3 U2", Status = 500 });

        GetLog sut = CreateSut(context);

        PaginationInput pagination = new () { Index = 0, Limit = 10 };

        // Act;
        (IEnumerable<Log> output, int count) = await sut.Execute(pagination, user1.UserId);

        // Assert;
        Assert.Equal(2, count);
        Assert.All(output, l => Assert.Equal(user1.UserId, l.UserId));
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenNoLogsExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetLog sut = CreateSut(context);

        PaginationInput pagination = new () { Index = 0, Limit = 10 };

        // Act;
        (IEnumerable<Log> output, int count) = await sut.Execute(pagination, null);

        // Assert;
        Assert.Empty(output);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Execute_ShouldReturnPaginatedResults()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        for (int i = 1; i <= 15; i++)
        {
            await Fixture.Save(context, new Log
            {
                LogId = Guid.NewGuid(),
                UserId = user.UserId,
                Description = $"Log {i}",
                Status = 200
            });
        }

        GetLog sut = CreateSut(context);
        PaginationInput pagination = new() { Index = 1, Limit = 5 };

        // Act;
        (IEnumerable<Log> output, int count) = await sut.Execute(pagination, user.UserId);

        // Assert;
        Assert.Equal(15, count);
        Assert.Equal(5, output.Count());
        Assert.Equal("Log 10", output.First().Description); // Assuming OrderByDescending;
    }

    #region helpers
    private static GetLog CreateSut(Context context)
    {
        GetLog getLog = new(context);

        return getLog;
    }
    #endregion
}
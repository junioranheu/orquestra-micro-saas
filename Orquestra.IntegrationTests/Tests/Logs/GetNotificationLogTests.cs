using Microsoft.Extensions.Caching.Memory;
using Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;
using Orquestra.Application.UseCases.Logs.GetNotification;
using Orquestra.Application.UseCases.Logs.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Logs;

public sealed class GetNotificationLogTests
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task Execute_ShouldReturnLogs_WhenCompanyIdMatchesOrUserIdMatches()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create("Junior Souza", "junior@test.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        string parametersWithCompany = $"{{\"CompanyId\":\"{company.CompanyId}\"}}";

        await Fixture.Save(context, new Log
        {
            LogId = Guid.NewGuid(),
            LogType = LogTypeEnum.Request,
            RequestType = "POST",
            Endpoint = "/api/Company",
            Parameters = parametersWithCompany,
            UserId = user.UserId,
            CreatedDate = GetDate().AddMinutes(-5)
        });

        await Fixture.Save(context, new Log
        {
            LogId = Guid.NewGuid(),
            LogType = LogTypeEnum.Exception,
            RequestType = "GET",
            Endpoint = "/api/Schedule",
            Parameters = parametersWithCompany,
            UserId = user.UserId,
            CreatedDate = GetDate().AddMinutes(-4)
        });

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true,
            IsCurrentMainCompanyUser = true
        };

        await Fixture.Save(context, companyUser);

        GetNotificationLog sut = CreateSut(context, _cache);
        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act;
        (List<LogNotificationOutput> output, int count) = await sut.Execute(pagination, user.UserId, false);

        // Assert;
        Assert.Equal(2, count);
        Assert.Equal(2, output.Count);
        Assert.Contains(output, o => o.EndpointName == "Empresa");
        Assert.Contains(output, o => o.EndpointName == "Agendamento");
        Assert.All(output, o => Assert.NotNull(o.Emoji));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserHasNoMainCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create("Junior Souza", "one@test.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        GetNotificationLog sut = CreateSut(context, _cache);
        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(pagination, user.UserId, false));
    }

    [Fact]
    public async Task Execute_ShouldReturnPaginatedResults()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create("Junior Souza", "junior@test.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true,
            IsCurrentMainCompanyUser = true
        };

        await Fixture.Save(context, companyUser);

        string parameters = $"{{\"CompanyId\":\"{company.CompanyId}\"}}";

        for (int i = 1; i <= 15; i++)
        {
            await Fixture.Save(context, new Log
            {
                LogId = Guid.NewGuid(),
                LogType = LogTypeEnum.Request,
                RequestType = "POST",
                Endpoint = "/api/Client",
                Parameters = parameters,
                UserId = user.UserId,
                Description = $"Log {i}",
                CreatedDate = GetDate().AddMinutes(-i)
            });
        }

        GetNotificationLog sut = CreateSut(context, _cache);
        PaginationInput pagination = new() { Index = 1, Limit = 5 };

        // Act;
        (List<LogNotificationOutput> output, int count) = await sut.Execute(pagination, user.UserId, false);

        // Assert;
        Assert.Equal(15, count);
        Assert.Equal(5, output.Count);
        Assert.All(output, x => Assert.Equal("Cliente", x.EndpointName));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserHasCompanyButNotMainCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User user = UserMock.Create("Junior Souza", "no-main@test.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true,
            IsCurrentMainCompanyUser = false
        };

        await Fixture.Save(context, companyUser);

        GetNotificationLog sut = CreateSut(context, _cache);
        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(pagination, user.UserId, false));
        Assert.Contains("você não está vinculado", exception.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserDoNotHaveCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create("Junior Souza", "no-main@test.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        GetNotificationLog sut = CreateSut(context, _cache);
        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(pagination, user.UserId, false));
        Assert.Contains("você não está vinculado", exception.Message);
    }

    #region helpers
    private static GetNotificationLog CreateSut(Context context, IMemoryCache cache)
    {
        GetCurrentMainCompanyUser getCurrentMainCompanyUser = new(context);
        GetNotificationLog getNotificationLog = new(context, cache, getCurrentMainCompanyUser);

        return getNotificationLog;
    }
    #endregion
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.GetFilter;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.GenericCache;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Clients;

public sealed class GetFilterClientTests
{
    [Fact]
    public async Task Execute_ShouldReturnFilters_WhenClientsExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client1 = new()
        {
            ClientId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Company = company,
            FullName = "Junior",
            Email = "junior@example.com"
        };

        Client client2 = new()
        {
            ClientId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Company = company,
            FullName = "Mariana",
            Email = "mariana@example.com"
        };

        await Fixture.Save(context, client1);
        await Fixture.Save(context, client2);

        GetFilterClient sut = CreateSut(context, user);

        // Act;
        ClientFilterOutput? result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.NotNull(result.FullNames);
        Assert.NotNull(result.Emails);

        Assert.Equal(2, result.FullNames!.Count);
        Assert.Equal(2, result.Emails!.Count);

        Assert.Contains(result.FullNames, x => x.Label == "Junior");
        Assert.Contains(result.FullNames, x => x.Label == "Mariana");
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenNoClientsFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetFilterClient sut = CreateSut(context, user);

        // Act;
        ClientFilterOutput? result = await sut.Execute(Guid.NewGuid(), Guid.NewGuid());

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result.FullNames!);
        Assert.Empty(result.Emails!);
    }

    #region helper
    private static GetFilterClient CreateSut(Context context, User user)
    {
        IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        IGenericCacheService cache = new GenericCacheService(memoryCache);
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);

        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        GetAllClientByCompanyId getAllClientByCompanyId = new(context, checkIfUserIsLinkedCompanyUser);

        GetFilterClient sut = new(cache, getAllClientByCompanyId);

        return sut;
    }
    #endregion
}
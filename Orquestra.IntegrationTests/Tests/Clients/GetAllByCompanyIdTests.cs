using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Clients;

public sealed class GetAllByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnClients_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client1 = ClientMock.Create();
        await Fixture.Save(context, client1);

        client1.CompanyId = company.CompanyId;
        context.Update(client1);
        await context.SaveChangesAsync();

        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);

        client2.CompanyId = company.CompanyId;
        context.Update(client2);
        await context.SaveChangesAsync();

       CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetClientByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ClientOutput>? result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.ClientId == client1.ClientId);
        Assert.Contains(result, r => r.ClientId == client2.ClientId);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenCompanyHasNoClients()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetClientByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ClientOutput>? result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Cria um outro usuário vinculado à empresa, mas não o user original;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetClientByCompanyId sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, company.CompanyId));
    }

    #region helper
    private static GetClientByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetClientByCompanyId getClientByCompanyId = new(context, checkIfUserIsLinkedCompanyUser);

        return getClientByCompanyId;
    }
    #endregion
}
using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.Delete;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Clients;

public sealed class DeleteClientTests
{
    [Fact]
    public async Task Execute_ShouldDeactivateClient_WhenUserIsAdministrator()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        DeleteClient sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, client.ClientId);

        // Assert;
        Client? deleted = await context.Clients.FindAsync(client.ClientId);
        Assert.NotNull(deleted);
        Assert.False(deleted.Status);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenClientDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        DeleteClient sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotAdministrator()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Cria o vínculo, mas com papel de Colaborador (não admin);
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member,
            Status = true
        };

        await Fixture.Save(context, companyUser);

        DeleteClient sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, client.ClientId));
    }

    #region helper
    private static DeleteClient CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        DeleteClient deleteClient = new(context, checkIfUserIsLinkedCompanyUser);

        return deleteClient;
    }
    #endregion
}
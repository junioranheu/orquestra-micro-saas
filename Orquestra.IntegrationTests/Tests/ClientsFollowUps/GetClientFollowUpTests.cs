using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.ClientsFollowUps.Get;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ClientsFollowUps;

public sealed class GetClientFollowUpTests
{
    [Fact]
    public async Task Execute_ShouldReturnFollowUps_WhenClientExistsAndUserLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Vincula o usuário à empresa do cliente;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        // Cria dois follow-ups;
        ClientFollowUp followUp1 = ClientFollowUpMock.Create(client);
        await Fixture.Save(context, followUp1);

        ClientFollowUp followUp2 = ClientFollowUpMock.Create(client);
        await Fixture.Save(context, followUp2);

        GetClientFollowUp sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientId = client.ClientId
        };

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.NotEmpty(output);
        Assert.Equal(2, count);
        Assert.All(output, o => Assert.Equal(client.ClientId, o.ClientId));
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenClientHasNoFollowUps()
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
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        GetClientFollowUp sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientId = client.ClientId
        };

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.Empty(output);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenClientNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetClientFollowUp sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientId = Guid.NewGuid()
        };

        // Act & Assert;
        ArgumentException ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
        Assert.Equal(SystemConsts.Warnings.NotFoundClient, ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Outro usuário vinculado, mas não o autenticado;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        ClientFollowUp followUp = ClientFollowUpMock.Create(client);
        await Fixture.Save(context, followUp);

        GetClientFollowUp sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientId = client.ClientId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    #region helper
    private static GetClientFollowUp CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAllByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getAllByCompanyId, httpContextAccessor);

        GetClientFollowUp getClientFollowUp = new(context, checkIfUserIsLinkedCompanyUser);

        return getClientFollowUp;
    }
    #endregion
}
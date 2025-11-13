using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Create;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ClientsFollowUps;

public sealed class CreateClientFollowUpTests
{
    [Fact]
    public async Task Execute_ShouldCreateClientFollowUp_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Vincula o usuário à mesma empresa do cliente;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        CreateClientFollowUp sut = CreateSut(context, user);

        ClientFollowUp clientFollowUp = ClientFollowUpMock.Create(client);
        ClientFollowUpInput input = clientFollowUp.Adapt<ClientFollowUpInput>();

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        ClientFollowUp? created = await context.ClientsFollowUps.AsNoTracking().FirstOrDefaultAsync(x => x.ClientFollowUpId == input.ClientFollowUpId);
        Assert.NotNull(created);
        Assert.Equal(input.ClientId, created.ClientId);
        Assert.Equal(input.Observation, created.Observation);
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

        // Outro user vinculado, mas não o user autenticado;
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

        CreateClientFollowUp sut = CreateSut(context, user);

        ClientFollowUp clientFollowUp = ClientFollowUpMock.Create(client);
        ClientFollowUpInput input = clientFollowUp.Adapt<ClientFollowUpInput>();

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    #region helper
    private static CreateClientFollowUp CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAllByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getAllByCompanyId, httpContextAccessor);

        CreateClientFollowUp createClientFollowUp = new(context, checkIfUserIsLinkedCompanyUser);

        return createClientFollowUp;
    }
    #endregion
}
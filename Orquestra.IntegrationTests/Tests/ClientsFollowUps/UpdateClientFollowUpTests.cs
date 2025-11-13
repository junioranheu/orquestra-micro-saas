using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.ClientsFollowUps.Update;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ClientsFollowUps;

public sealed class UpdateClientFollowUpTests
{
    [Fact]
    public async Task Execute_ShouldUpdateClientFollowUp_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Vincula o user à mesma empresa do client;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = client.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        // Cria um follow-up existente;
        ClientFollowUp clientFollowUp = ClientFollowUpMock.Create(client);
        await Fixture.Save(context, clientFollowUp);

        UpdateClientFollowUp sut = CreateSut(context, user);

        // Cria input com os novos dados;
        ClientFollowUpInput input = clientFollowUp.Adapt<ClientFollowUpInput>();
        input.Observation = "Cliente reagendou para próxima semana.";
        input.ClientFollowUpStatus = ClientFollowUpStatusEnum.Completed;

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        ClientFollowUp? updated = await context.ClientsFollowUps.AsNoTracking().FirstOrDefaultAsync(x => x.ClientFollowUpId == clientFollowUp.ClientFollowUpId);
        Assert.NotNull(updated);
        Assert.Equal(input.Observation, updated.Observation);
        Assert.Equal(input.ClientFollowUpStatus, updated.ClientFollowUpStatus);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenClientFollowUpNotFound()
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

        UpdateClientFollowUp sut = CreateSut(context, user);

        ClientFollowUpInput input = new()
        {
            ClientFollowUpId = Guid.NewGuid(),
            ClientId = client.ClientId,
            Observation = "Tentativa de atualizar inexistente.",
            ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress
        };

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
        Assert.Equal(SystemConsts.Warnings.NotFoundData, ex.Message);
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

        // Outro usuário vinculado à empresa, mas não o autenticado;
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

        ClientFollowUp clientFollowUp = ClientFollowUpMock.Create(client);
        await Fixture.Save(context, clientFollowUp);

        UpdateClientFollowUp sut = CreateSut(context, user);

        ClientFollowUpInput input = clientFollowUp.Adapt<ClientFollowUpInput>();
        input.Observation = "Tentativa não autorizada.";

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    #region helper
    private static UpdateClientFollowUp CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAllByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getAllByCompanyId, httpContextAccessor);

        UpdateClientFollowUp updateClientFollowUp = new(context, checkIfUserIsLinkedCompanyUser);

        return updateClientFollowUp;
    }
    #endregion
}
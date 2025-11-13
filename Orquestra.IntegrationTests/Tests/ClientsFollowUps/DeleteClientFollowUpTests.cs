using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.ClientsFollowUps.Delete;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ClientsFollowUps;

public sealed class DeleteClientFollowUpTests
{
    [Fact]
    public async Task Execute_ShouldPass_WhenClientFollowUpExistsAndUserIsAuthorized()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        client.CompanyId = company.CompanyId;
        await Fixture.Save(context, client);

        ClientFollowUp followUp = ClientFollowUpMock.Create();
        followUp.ClientId = client.ClientId;
        followUp.Status = true;
        await Fixture.Save(context, followUp);

        DeleteClientFollowUp sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, followUp.ClientFollowUpId);

        // Assert;
        ClientFollowUp? deleted = await context.ClientsFollowUps.AsNoTracking()  .FirstOrDefaultAsync(x => x.ClientFollowUpId == followUp.ClientFollowUpId);

        Assert.NotNull(deleted);
        Assert.False(deleted!.Status);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenClientFollowUpNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        DeleteClientFollowUp sut = CreateSut(context, user);

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid()));
        Assert.Equal(SystemConsts.Warnings.NotFoundData, ex.Message);
    }

    #region helper
    private static DeleteClientFollowUp CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        DeleteClientFollowUp deleteClientFollowUp = new(context, checkIfUserIsLinkedCompanyUser);

        return deleteClientFollowUp;
    }
    #endregion
}
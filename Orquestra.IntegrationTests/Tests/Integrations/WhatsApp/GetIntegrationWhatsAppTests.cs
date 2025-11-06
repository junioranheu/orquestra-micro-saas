using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Get;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Integrations.WhatsApp;

public sealed class GetIntegrationWhatsAppTests
{
    [Fact]
    public async Task Execute_ShouldReturnIntegration_WhenExistsAndActive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        IntegrationWhatsApp integration = new()
        {
            CompanyId = company.CompanyId,
            Status = true
        };

        await Fixture.Save(context, integration);

        GetIntegrationWhatsApp sut = CreateSut(context, user);

        // Act;
        IntegrationWhatsApp result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(company.CompanyId, result.CompanyId);
        Assert.True(result.Status);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenIntegrationDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        GetIntegrationWhatsApp sut = CreateSut(context, user);

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, company.CompanyId));

        Assert.Contains(SystemConsts.Warnings.NotFoundData, ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenIntegrationIsInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        IntegrationWhatsApp integration = new()
        {
            CompanyId = company.CompanyId
        };

        await Fixture.Save(context, integration);

        integration.Status = false;
        context.Update(integration);
        await context.SaveChangesAsync();

        GetIntegrationWhatsApp sut = CreateSut(context, user);

        // Act & Assert;
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, company.CompanyId));

        Assert.Contains(SystemConsts.Warnings.NotFoundData, ex.Message);
    }

    #region helper
    private static GetIntegrationWhatsApp CreateSut(Context context, User user)
    {
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, Fixture.CreateIHttpContextAccessor(user));

        GetIntegrationWhatsApp getIntegrationWhatsApp = new(context, checkIfUserIsLinkedCompanyUser);

        return getIntegrationWhatsApp;
    }
    #endregion
}
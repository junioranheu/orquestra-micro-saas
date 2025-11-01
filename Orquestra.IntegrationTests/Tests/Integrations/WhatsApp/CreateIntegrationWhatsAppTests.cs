using Microsoft.EntityFrameworkCore;
using Moq;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Create;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Sms;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Integrations.WhatsApp;

public sealed class CreateIntegrationWhatsAppTests
{
    [Fact]
    public async Task Execute_ShouldCreateIntegration_WhenInputIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Premium;
        company.CompanySituation = CompanySituationEnum.Approved;
        await Fixture.Save(context, company);

        CreateIntegrationWhatsApp sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        IntegrationWhatsApp? result = await context.IntegrationsWhatsApp.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == company.CompanyId);

        Assert.NotNull(result);
        Assert.Equal(company.CompanyId, result!.CompanyId);
    }

    [Fact]
    public async Task Execute_ShouldDeletePreviousIntegrations_WhenInputIsNotNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Premium;
        company.CompanySituation = CompanySituationEnum.Approved;
        await Fixture.Save(context, company);

        IntegrationWhatsApp oldIntegration = new() { CompanyId = company.CompanyId };
        await Fixture.Save(context, oldIntegration);

        CreateIntegrationWhatsApp sut = CreateSut(context, user);

        IntegrationWhatsApp newIntegration = new() { CompanyId = company.CompanyId };

        // Act;
        await sut.Execute(user.UserId, company.CompanyId, newIntegration);

        // Assert;
        List<IntegrationWhatsApp> integrations = await context.IntegrationsWhatsApp.AsNoTracking().ToListAsync();
        Assert.Single(integrations);
        Assert.Equal(company.CompanyId, integrations.First().CompanyId);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyPlanIsFree()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Free;
        company.CompanySituation = CompanySituationEnum.Approved;
        await Fixture.Save(context, company);

        CreateIntegrationWhatsApp sut = CreateSut(context, user);

        IntegrationWhatsApp input = new() { CompanyId = company.CompanyId };

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId, input));

        Assert.Contains("whatsapp", ex.Message.ToLowerInvariant());
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanySituationIsPendingPayment()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Premium;
        company.CompanySituation = CompanySituationEnum.PendingPayment;
        await Fixture.Save(context, company);

        CreateIntegrationWhatsApp sut = CreateSut(context, user);

        IntegrationWhatsApp input = new() { CompanyId = company.CompanyId };

        // Act & Assert;
        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId, input));

        Assert.Contains("situação", ex.Message.ToLowerInvariant());
    }

    #region helper
    private static CreateIntegrationWhatsApp CreateSut(Context context, User user)
    {
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, Fixture.CreateIHttpContextAccessor(user));

        Mock<ISmsService> smsServiceMock = new();
        smsServiceMock.Setup(x => x.SendSms(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>())).ReturnsAsync("OK");

        IntegrationWhatsAppBaseDependencies deps = new(context, checkIfUserIsLinkedCompanyUser, smsServiceMock.Object);

        CreateIntegrationWhatsApp createIntegrationWhatsApp = new(deps);

        return createIntegrationWhatsApp;
    }
    #endregion
}
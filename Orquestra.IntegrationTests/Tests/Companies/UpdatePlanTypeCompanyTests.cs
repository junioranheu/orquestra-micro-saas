using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Orquestra.Application.UseCases.Companies.UpdatePlanType;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Companies;

public sealed class UpdatePlanTypeCompanyTests
{
    [Fact]
    public async Task Should_UpdateCompanyPlanAndCreateInvoice()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        Mock<IEmailService> emailServiceMock = Fixture.CreateEmailService();

        var sut = CreateSut(context, user, emailServiceMock);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Orquestra LTDA",
            PlanType = PlanTypeEnum.Basic,
            Status = true,
            CompanySituation = CompanySituationEnum.PendingPayment,
            PlanStartDate = GetDate(),
            PlanEndDate = GetDate().AddDays(30)
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act;
        await sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Premium);

        // Assert;
        Company updatedCompany = await context.Companies.FirstAsync(x => x.CompanyId == company.CompanyId);

        Assert.Equal(CompanySituationEnum.PendingPayment, updatedCompany.CompanySituation);
        Assert.Equal(PlanTypeEnum.Premium, updatedCompany.PlanType);
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, Guid.NewGuid(), PlanTypeEnum.Premium));
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyIsInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Inactive Company",
            PlanType = PlanTypeEnum.Basic,
            Status = false
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        company.Status = false;
        context.Update(company);
        await context.SaveChangesAsync();

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Premium));
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyAlreadyHasPlan()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Test Company",
            PlanType = PlanTypeEnum.Basic,
            Status = true
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Basic));
    }

    [Fact]
    public async Task Should_AdjustDates_WhenPlanStartDateIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Test Company",
            PlanType = PlanTypeEnum.Basic,
            Status = true,
            PlanStartDate = null,
            PlanEndDate = null
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act;
        await sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Premium);

        // Assert;
        Company updated = await context.Companies.FirstAsync(x => x.CompanyId == company.CompanyId);
        Assert.Null(updated.PlanStartDate);
        Assert.Null(updated.PlanEndDate);
        Assert.Equal(CompanySituationEnum.PendingPayment, updated.CompanySituation);
        Assert.Equal(PlanTypeEnum.Premium, updated.PlanType);
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyAlreadyHasSamePlan()
    {
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Company Same Plan",
            PlanType = PlanTypeEnum.Basic,
            Status = true
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Basic));

        Assert.Contains("já está com o plano", ex.Message.ToLowerInvariant());
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyHasPendingInvoice()
    {
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Company With Pending Invoice",
            PlanType = PlanTypeEnum.Basic,
            Status = true
        };

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Premium,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending,
            Status = true
        };

        context.Companies.Add(company);
        context.CompanyInvoices.Add(invoice);
        await context.SaveChangesAsync();

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>     sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Premium));

        Assert.Contains("fatura em aberto", ex.Message);
    }

    [Fact]
    public async Task Should_PassCheckAvailability_WhenPlanIsDifferentAndNoPendingInvoices()
    {
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();
        UpdatePlanTypeCompany sut = CreateSut(context, user);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Company Free to Upgrade",
            PlanType = PlanTypeEnum.Basic,
            Status = true
        };

        context.Companies.Add(company);
        await context.SaveChangesAsync();

        await sut.Execute(user.UserId, company.CompanyId, PlanTypeEnum.Premium);

        Company updated = await context.Companies.FirstAsync(x => x.CompanyId == company.CompanyId);
        Assert.Equal(PlanTypeEnum.Premium, updated.PlanType);
        Assert.Equal(CompanySituationEnum.PendingPayment, updated.CompanySituation);
    }

    #region helper
    private static UpdatePlanTypeCompany CreateSut(Context context, User user, Mock<IEmailService>? emailServiceMock = null)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        IWebHostEnvironment env = Fixture.CreateDevelopmentEnvironment();
        IConfiguration config = Fixture.CreateConfiguration();
        emailServiceMock ??= Fixture.CreateEmailService();
        EnvService envService = new(env, config);

        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        CreateCompanyInvoice createCompanyInvoice = new(context, checkIfUserIsLinkedCompanyUser, envService, emailServiceMock.Object);

        UpdatePlanTypeCompany updatePlanTypeCompany = new(context, checkIfUserIsLinkedCompanyUser, createCompanyInvoice);

        return updatePlanTypeCompany;
    }
    #endregion
}
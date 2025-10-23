using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyInvoices.Create;
using Orquestra.Application.UseCases.CompanyInvoices.Pay;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.CompanyInvoices;

public sealed class PayCompanyInvoiceTests
{
    [Fact]
    public async Task Should_PayInvoiceAndUpdateCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        PayCompanyInvoice sut = CreateSut(context, adminUser);

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            Name = "Company Test",
            PlanType = PlanTypeEnum.Basic,
            Status = true,
            CompanySituation = CompanySituationEnum.PendingPayment,
            PlanStartDate = null,
            PlanEndDate = null
        };

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Basic,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending,
            Status = true
        };

        context.Companies.Add(company);
        context.CompanyInvoices.Add(invoice);
        await context.SaveChangesAsync();

        // Act;
        await sut.Execute(Guid.NewGuid(), invoice.CompanyInvoiceId);

        // Assert;
        Company updatedCompany = await context.Companies.FirstAsync(x => x.CompanyId == company.CompanyId);
        CompanyInvoice updatedInvoice = await context.CompanyInvoices.FirstAsync(x => x.CompanyInvoiceId == invoice.CompanyInvoiceId);

        Assert.Equal(CompanySituationEnum.Approved, updatedCompany.CompanySituation);
        Assert.NotNull(updatedCompany.PlanStartDate);
        Assert.NotNull(updatedCompany.PlanEndDate);

        Assert.Equal(CompanyInvoiceSituationEnum.Paid, updatedInvoice.CompanyInvoiceSituation);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Basic,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending,
            Status = true
        };

        await Fixture.Save(context, invoice);

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

        PayCompanyInvoice sut = CreateSut(context, adminUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(adminUser.UserId, invoice.CompanyInvoiceId));
    }

    [Fact]
    public async Task Should_ThrowException_WhenInvoiceDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        PayCompanyInvoice sut = CreateSut(context, adminUser);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User adminUser = UserMock.Create();
        await Fixture.Save(context, adminUser);

        PayCompanyInvoice sut = CreateSut(context, adminUser);

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(), // Empresa não existe;
            PlanType = PlanTypeEnum.Basic,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending,
            Status = true
        };

        context.CompanyInvoices.Add(invoice);
        await context.SaveChangesAsync();

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(Guid.NewGuid(), invoice.CompanyInvoiceId));
    }

    #region helper
    private static PayCompanyInvoice CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);
        PayCompanyInvoice payCompanyInvoice = new (context, checkIfUserIsLinkedCompanyUser);

        return payCompanyInvoice;
    }
    #endregion
}
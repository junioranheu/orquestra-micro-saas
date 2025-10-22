using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyInvoices.Pay;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;

namespace Orquestra.IntegrationTests.Tests.CompanyInvoices;

public sealed class PayCompanyInvoiceTests
{
    [Fact]
    public async Task Should_PayInvoiceAndUpdateCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        var sut = CreateSut(context);

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
    public async Task Should_ThrowException_WhenInvoiceDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        var sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Should_ThrowException_WhenCompanyDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        var sut = CreateSut(context);

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
    private static PayCompanyInvoice CreateSut(Context context)
    {
        PayCompanyInvoice payCompanyInvoice = new (context);

        return payCompanyInvoice;
    }
    #endregion
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Jobs;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Tests.Jobs;

public sealed class CompanyPlanJobTests
{
    [Fact]
    public async Task Should_UpdateExpiredCompaniesAndInvoices()
    {
        // Arrange;
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<Context>().UseSqlite(connection).Options;

        using var context = new Context(options);
        context.Database.EnsureCreated();

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            CompanySituation = CompanySituationEnum.Approved,
            PlanEndDate = GetDate().AddDays(-2),
            Status = true
        };

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Basic,
            Amount = 99,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid
        };

        await context.AddRangeAsync(company, invoice);
        await context.SaveChangesAsync();

        CompanyPlanJob sut = CreateSut(context);

        // Act;
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(2));
        await sut.StartAsync(cts.Token);
        await sut.StopAsync(CancellationToken.None);

        // Assert;
        Company updatedCompany = await context.Companies.FirstAsync();
        CompanyInvoice updatedInvoice = await context.CompanyInvoices.FirstAsync();

        Assert.Equal(CompanySituationEnum.PendingPayment, updatedCompany.CompanySituation);
        Assert.Equal(CompanyInvoiceSituationEnum.Expired, updatedInvoice.CompanyInvoiceSituation);

        Log? log = await context.Logs.FirstOrDefaultAsync();
        Assert.NotNull(log);
        Assert.Contains("Empresas atualizadas", log.Description);
    }

    [Fact]
    public async Task Should_NotUpdate_WhenCompanyStillActive()
    {
        // Arrange;
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<Context>().UseSqlite(connection).Options;

        using var context = new Context(options);
        context.Database.EnsureCreated();

        Company company = new()
        {
            CompanyId = Guid.NewGuid(),
            CompanySituation = CompanySituationEnum.Approved,
            PlanEndDate = GetDate().AddDays(5),
            Status = true
        };

        CompanyInvoice invoice = new()
        {
            CompanyInvoiceId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            PlanType = PlanTypeEnum.Basic,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid
        };

        await context.AddRangeAsync(company, invoice);
        await context.SaveChangesAsync();

        CompanyPlanJob sut = CreateSut(context);

        // Act;
        using CancellationTokenSource cts = new(TimeSpan.FromSeconds(2));
        await sut.StartAsync(cts.Token);
        await sut.StopAsync(CancellationToken.None);

        // Assert;
        Company unchangedCompany = await context.Companies.FirstAsync();
        CompanyInvoice unchangedInvoice = await context.CompanyInvoices.FirstAsync();

        Assert.Equal(CompanySituationEnum.Approved, unchangedCompany.CompanySituation);
        Assert.Equal(CompanyInvoiceSituationEnum.Paid, unchangedInvoice.CompanyInvoiceSituation);
        Assert.Empty(context.Logs);
    }


    [Fact]
    public async Task Should_CreateLogSuccessfully()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        string description = "Log de teste - CompanyPlanJob";

        // Act;
        await CompanyPlanJob.CreateLog(context, LoggerFactory.Create(builder => { }).CreateLogger<CompanyPlanJob>(), description);

        // Assert;
        Log? log = await context.Logs.FirstOrDefaultAsync();

        Assert.NotNull(log);
        Assert.Equal(LogTypeEnum.Job, log.LogType);
        Assert.Equal(description, log.Description);
    }

    #region helper
    private static CompanyPlanJob CreateSut(Context context)
    {
        ServiceCollection services = new();
        services.AddScoped(_ => context);
        ServiceProvider provider = services.BuildServiceProvider();

        IServiceScopeFactory scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
        ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<CompanyPlanJob> logger = loggerFactory.CreateLogger<CompanyPlanJob>();

        CompanyPlanJob companyPlanJob = new(scopeFactory, logger);

        return companyPlanJob;
    }
    #endregion
}
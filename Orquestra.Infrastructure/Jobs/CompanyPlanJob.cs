using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Jobs;

public sealed class CompanyPlanJob(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            Context context = scope.ServiceProvider.GetRequiredService<Context>();

            // Início;
            await CheckAndExpirePlans(context);

            // Loop;
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    #region obsolete
    //private static async Task CheckAndExpirePlans(Context context)
    //{
    //    List<Guid> expiredCompanyIds = await context.Companies.Where(c => c.PlanEndDate <= GetDate() && c.Status == true).Select(c => c.CompanyId).ToListAsync();

    //    if (expiredCompanyIds.Count == 0)
    //    {
    //        return;
    //    }

    //    HashSet<Guid> expiredCompanyIdsSet = [.. expiredCompanyIds];

    //    // Atualiza empresas para pendente de pagamento;
    //    await context.Companies.Where(x => expiredCompanyIdsSet.Contains(x.CompanyId)).ExecuteUpdateAsync(x => x.SetProperty(x => x.CompanySituation, CompanySituationEnum.PendingPayment));

    //    // Atualiza invoices relacionados para expirados;
    //    await context.CompanyInvoices.Where(x => expiredCompanyIdsSet.Contains(x.CompanyId) && x.Status == true).ExecuteUpdateAsync(x => x.SetProperty(x => x.CompanyInvoiceSituation, CompanyInvoiceSituationEnum.Expired));
    //}
    #endregion

    private static async Task CheckAndExpirePlans(Context context)
    {
        DateTime now = GetDate();
        int pendingPaymentValue = (int)CompanySituationEnum.PendingPayment;

        // Atualiza empresas para PendingPayment;
        string companiesSql = @"
        UPDATE ""Companies""
        SET ""CompanySituation"" = {0}
        WHERE ""PlanEndDate"" <= {1}
        AND ""CompanySituation"" != {0};";

        int companiesUpdated = await context.Database.ExecuteSqlRawAsync(
            companiesSql,           
            pendingPaymentValue,    // {0} = pendingPaymentValue ;
            now                     // {1} = now;
        );

        Console.WriteLine($"Companies updated: {companiesUpdated}");

        if (companiesUpdated > 0)
        {
            int expiredValue = (int)CompanyInvoiceSituationEnum.Expired;

            string invoicesSql = @"
            UPDATE ""CompanyInvoices"" ci
            SET ""CompanyInvoiceSituation"" = {0}
            FROM ""Companies"" c
            WHERE ci.""CompanyId"" = c.""CompanyId""
            AND ci.""CompanyInvoiceSituation"" != {0}
            AND c.""PlanEndDate"" <= {1}
            AND c.""CompanySituation"" = {2};";

            int invoicesUpdated = await context.Database.ExecuteSqlRawAsync(
                invoicesSql,
                expiredValue,        // {0}
                now,                 // {1}
                pendingPaymentValue  // {2}, apenas invoices de empresas que acabaram de virar PendingPayment;
            );

            Console.WriteLine($"Invoices updated: {invoicesUpdated}");
        }
    }
}
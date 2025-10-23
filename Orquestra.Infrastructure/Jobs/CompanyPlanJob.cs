using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Infrastructure.Jobs;

public sealed class CompanyPlanJob(IServiceScopeFactory scopeFactory, ILogger<CompanyPlanJob> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger _logger = logger;

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

    private async Task CheckAndExpirePlans(Context context)
    {
        DateTime now = GetDate();
        int pendingPaymentValue = (int)CompanySituationEnum.PendingPayment;

        // Atualiza empresas para CompanySituationEnum.PendingPayment;
        string companiesSql = @"
        UPDATE ""Companies""
        SET ""CompanySituation"" = {0}      -- {0} = novo status: CompanySituationEnum.PendingPayment
        WHERE ""PlanEndDate"" <= {1}        -- {1} = now
          AND ""CompanySituation"" != {0}   -- só empresas que ainda não estão PendingPayment
          AND ""Status"" = true;            -- só empresas ativas
        ";

        int companiesUpdated = await context.Database.ExecuteSqlRawAsync(
            companiesSql,
            pendingPaymentValue,    // {0} = novo status: CompanySituationEnum.PendingPayment;
            now                     // {1} = now;
        );

        // Atualiza os invoices para CompanyInvoiceSituationEnum.Expired;
        if (companiesUpdated > 0)
        {
            await CreateLog(context, _logger, description: $"Empresas atualizadas: {companiesUpdated}");

            int expiredValue = (int)CompanyInvoiceSituationEnum.Expired;

            string invoicesSql = @"
            UPDATE ""CompanyInvoices"" ci
            SET ""CompanyInvoiceSituation"" = {0}       -- {0} = novo status: CompanyInvoiceSituationEnum.Expired
            FROM ""Companies"" c
            WHERE ci.""CompanyId"" = c.""CompanyId""
              AND ci.""Status"" = true                  -- só invoices ativas
              AND ci.""CompanyInvoiceSituation"" != {0} -- só invoices que ainda não estão Expired
              AND c.""PlanEndDate"" <= {1}              -- {1} = now
              AND c.""CompanySituation"" = {2};         -- {2} = pendingPaymentValue
            ";

            int invoicesUpdated = await context.Database.ExecuteSqlRawAsync(
                invoicesSql,
                expiredValue,        // {0} novo status: CompanyInvoiceSituationEnum.Expired;
                now,                 // {1} now;
                pendingPaymentValue  // {2}, apenas invoices de empresas que acabaram de virar PendingPayment;
            );

            if (invoicesUpdated > 0)
            {
                await CreateLog(context, logger, description: $"Faturas atualizadas: {invoicesUpdated}");
            }            
        }
    }

    public static async Task CreateLog(Context context, ILogger logger, string description)
    {
        Log log = new()
        {
            LogType = LogTypeEnum.Job,
            RequestType = "POST",
            Endpoint =  nameof(CompanyPlanJob),
            Parameters = string.Empty,
            Exception = string.Empty,
            Description = description,
            Status = StatusCodes.Status204NoContent,
            UserId = null
        };

        logger.LogInformation("{description}", description);
        await context.AddAsync(log);
        await context.SaveChangesAsync();
    }
}
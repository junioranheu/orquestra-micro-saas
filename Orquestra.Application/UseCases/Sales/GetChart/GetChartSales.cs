using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public sealed class GetChartSales(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetChartSales
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<SalesOutput> Execute(PaginationInput pagination, Guid userIdAuth, Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(companyId)} está vazio.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);
        await ValidateCurrentPlan(companyId);

        List<SalesTableOutput> table = [];

        await GetDataFromInventory(table, companyId);
        await GetDataFromSchedule(table, companyId);

        if (table is null || table.Count == 0)
        {
            throw new KeyNotFoundException("Nenhum registro foi encontrado na base de dados para montar os gráficos e tabela da gestão financeira da sua empresa.");
        }

        List<SalesChartOutput> chartOutput = GetChartOutput(table);
        List<SalesTableOutput> tableOutput = GetTableOutput(table, pagination);
        (decimal cashInflow, decimal cashOutflow, decimal finalBalance) = GetCashFlowSummary(chartOutput);

        SalesOutput output = new()
        {
            Table = tableOutput,
            TableTotalCount = table.Count,
            Chart = chartOutput,
            CashInflow = cashInflow,
            CashOutflow = cashOutflow,
            FinalBalance = finalBalance
        };

        return output;
    }

    #region extras
    private async Task ValidateCurrentPlan(Guid companyId)
    {
        Company company = await _context.Companies.AsNoTracking().Where(x => x.CompanyId == companyId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        if (company.PlanType == PlanTypeEnum.Premium)
        {
            return;
        }

        throw new InvalidOperationException($"Para acessar o módulo <b>{GetEnumDesc(ModuleEnum.Sales).ToLowerInvariant()}</b> é necessário ter o plano <b>{GetEnumDesc(PlanTypeEnum.Premium).ToLowerInvariant()}</b> ativo.");
    }

    private async Task GetDataFromInventory(List<SalesTableOutput> table, Guid companyId)
    {
        var items = await _context.Inventories.
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        x.UnitPrice != 0
                    ).
                    Select(x => new SalesTableOutput
                    {
                        Id = Guid.NewGuid(),
                        Type = GetEnumDesc(ModuleEnum.Inventory),
                        Title = x.Name ?? string.Empty,
                        Description = $"Item de estoque criado em {ConvertToBrasiliaTime(x.CreatedDate.GetValueOrDefault())}",
                        Value = -(x.TotalValue) ?? 0, // Sempre o valor negativo;
                        Date = x.CreatedDate
                    }).
                    ToListAsync();

        if (items is null || items.Count == 0)
        {
            return;
        }

        table.AddRange(items);
    }

    private async Task GetDataFromSchedule(List<SalesTableOutput> table, Guid companyId)
    {
        var items = await _context.Schedules.
                    Include(x => x.Client).
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        x.AmountReceived > 0
                    ).
                    Select(x => new SalesTableOutput
                    {
                        Id = Guid.NewGuid(),
                        Type = GetEnumDesc(ModuleEnum.Scheduling),
                        Title = x.CustomTitle ?? $"Agendamento {(x.Client != null && !string.IsNullOrEmpty(x.Client.FullName) ? $"• {x.Client.FullName}" : string.Empty)}",
                        Description = $"Agendamento criado em {ConvertToBrasiliaTime(x.CreatedDate.GetValueOrDefault())}",
                        Value = x.AmountReceived ?? 0,
                        Date = x.CreatedDate
                    }).
                    ToListAsync();

        if (items is null || items.Count == 0)
        {
            return;
        }

        table.AddRange(items);
    }

    private static List<SalesChartOutput> GetChartOutput(List<SalesTableOutput> table)
    {
        string inventory = GetEnumDesc(ModuleEnum.Inventory);
        string scheduling = GetEnumDesc(ModuleEnum.Scheduling);

        List<SalesChartOutput> output = [.. table.
            GroupBy(x => x.Type).
            Select(g => new SalesChartOutput
            {
                Type = g.Key,
                Color = g.Key switch
                {
                   _ when g.Key == inventory => "var(--contrast)",
                   _ when g.Key == scheduling => "var(--main)",
                   _ => "var(--gray-dark)"
                },
                Items = [.. g.Select(x => new SalesChartItemOutput
                {
                    DateTime = x.Date?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                    Value = x.Value
                })]
            })];

        return output;
    }

    private static List<SalesTableOutput> GetTableOutput(List<SalesTableOutput> table, PaginationInput pagination)
    {
        List<SalesTableOutput> output = [.. table.
                                        OrderByDescending(x => x.Date).
                                        Skip(pagination.IsSelectAll ? 0 : pagination.Index * pagination.Limit).
                                        Take(pagination.IsSelectAll ? int.MaxValue : pagination.Limit)];

        return output;
    }

    private static (decimal cashInflow, decimal cashOutflow, decimal finalBalance) GetCashFlowSummary(List<SalesChartOutput> chartOutput)
    {
        decimal cashInflow = chartOutput.SelectMany(x => x.Items).Where(x => x.Value > 0).Sum(x => x.Value);
        decimal cashOutflow = chartOutput.SelectMany(x => x.Items).Where(x => x.Value < 0).Sum(x => x.Value);
        decimal finalBalance = cashInflow - Math.Abs(cashOutflow);

        return (cashInflow, cashOutflow, finalBalance);
    }
    #endregion
}
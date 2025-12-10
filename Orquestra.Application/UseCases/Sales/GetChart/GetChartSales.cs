using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public sealed class GetChartSales(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetChartSales
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<SalesOutput> Execute(Guid userIdAuth, Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(companyId)} está vazio.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        List<SalesTableOutput> table = [];

        await GetDataFromInventory(table, companyId);
        await GetDataFromSchedule(table, companyId);
        await GetDataFromServiceOrder(table, companyId);

        if (table is null || table.Count == 0)
        {
            throw new KeyNotFoundException("Nenhum registro foi encontrado na base de dados para montar os gráficos e tabela da gestão financeira da sua empresa.");
        }

        List<SalesChartOutput> chart = GetOutput(table);

        SalesOutput output = new()
        {
            Table = [.. table.OrderByDescending(x => x.Date)],
            Chart = chart
        };

        return output;
    }

    #region extras
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
                        Description = $"Item de estoque criado em {x.CreatedDate}",
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
                        Description = $"Agendamento criado em {x.CreatedDate}",
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

    private async Task GetDataFromServiceOrder(List<SalesTableOutput> table, Guid companyId)
    {
        // TO DO;
    }

    private static List<SalesChartOutput> GetOutput(List<SalesTableOutput> table)
    {
        string inventory = GetEnumDesc(ModuleEnum.Inventory);
        string scheduling = GetEnumDesc(ModuleEnum.Scheduling);
        string serviceOrder = GetEnumDesc(ModuleEnum.ServiceOrder);

        List<SalesChartOutput> chart = [.. table.
            GroupBy(x => x.Type).
            Select(g => new SalesChartOutput
            {
                Type = g.Key,
                Color = g.Key switch
                {
                   _ when g.Key == inventory => "var(--contrast)",
                   _ when g.Key == scheduling => "var(--contrast)",
                   _ when g.Key == serviceOrder => "var(--contrast)",
                   _ => "var(--main)"
                },
                Items = [.. g.Select(x => new SalesChartItemOutput
                {
                    DateTime = x.Date?.ToString("yyyy-MM-ddTHH:mm:ss") ?? string.Empty,
                    Value = x.Value
                })]
            })];

        return chart;
    }
    #endregion
}
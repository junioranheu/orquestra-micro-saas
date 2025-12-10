using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public sealed class GetChartSales(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IGetChartSales
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<List<SalesChartOutput>> Execute(Guid userIdAuth, Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(companyId)} está vazio.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        List<SalesChartOutput> output = [];

        await GetDataFromInventory(output, companyId);
        await GetDataFromSchedule(output, companyId);
        await GetDataFromServiceOrder(output, companyId);

        return output;
    }

    #region extras
    private async Task GetDataFromInventory(List<SalesChartOutput> output, Guid companyId)
    {
        var items = await _context.Inventories.
                    AsNoTracking().
                    Where(x => 
                        x.CompanyId == companyId && 
                        x.Status == true &&
                        x.UnitPrice != 0
                    ).
                    Select(x => new SalesChartOutput
                    {
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

        output.AddRange(items);
    }

    private async Task GetDataFromSchedule(List<SalesChartOutput> output, Guid companyId)
    {
        var items = await _context.Schedules.
                    Include(x => x.Client).
                    AsNoTracking().
                    Where(x => 
                        x.CompanyId == companyId && 
                        x.Status == true &&
                        x.AmountReceived > 0
                    ).
                    Select(x => new SalesChartOutput
                    {
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

        output.AddRange(items);
    }

    private async Task GetDataFromServiceOrder(List<SalesChartOutput> output, Guid companyId)
    {
        // TO DO;
    }
    #endregion
}
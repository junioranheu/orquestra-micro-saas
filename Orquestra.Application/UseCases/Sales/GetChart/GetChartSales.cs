using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Sales.GetChart;

public sealed class GetChartSales(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser)
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<SalesChartOutput> Execute(Guid userIdAuth, Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException($"O parâmetro {nameof(companyId)} está vazio.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        SalesChartOutput output = new();

        await GetDataFromInventory(companyId);
        await GetDataFromSchedule(companyId);
        await GetDataFromServiceOrder(companyId);

        return output;
    }

    #region extras
    private async Task GetDataFromInventory(Guid companyId)
    {
        var output = await _context.Inventories.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();
    }


    private async Task GetDataFromSchedule(Guid companyId)
    {
        var output = await _context.Schedules.
                     AsNoTracking().
                     Where(x => x.CompanyId == companyId && x.Status == true).
                     ToListAsync();
    }

    private async Task GetDataFromServiceOrder(Guid companyId)
    {
        // TO DO;
    }
    #endregion
}
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyInvoices.Create;

public sealed class CreateCompanyInvoice(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ICreateCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyInvoice?> Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules)
    {
        if (modules is null || modules.Length == 0)
        {
            return null;
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        (Company? company, ModuleEnum[] newModules) = await CheckIfCompanyAlreadyHasModuleOrInvoice(companyId, modules);

        if (newModules is null || newModules.Length == 0)
        {
            return null;
        }

        List<string> modulesStr = [];
        decimal amount = 0;

        foreach (var item in modules)
        {
            modulesStr.Add(GetEnumDesc(item));

            decimal price = ModuleHelper.GetPrice(item);

            // TO DO LOGICA DE NAO COBRAR TUDO, E SIM SÓ OS DIAS FALTANTES;

            amount += price;
        }

        CompanyInvoice invoice = new()
        {
            CompanyId = companyId,
            Modules = modules,
            Amount = amount,
            Description = modules.Length > 1 ? $"Adição dos módulos: {string.Join("; ", modulesStr)}" : $"Adição do módulo: {string.Join("; ", modulesStr)}",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending
        };

        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    #region extras
    private async Task<(Company? company, ModuleEnum[] newModules)> CheckIfCompanyAlreadyHasModuleOrInvoice(Guid companyId, ModuleEnum[] modules)
    {
        Company? company = await _context.Companies.AsNoTracking().Where(x => x.CompanyId == companyId && x.Status == true).FirstOrDefaultAsync();

        if (company is null)
        {
            return (null, []);
        }

        HashSet<ModuleEnum> existingCompanyModules = [.. company.Modules ?? []];
        ModuleEnum[] newModules = [.. modules.Where(x => !existingCompanyModules.Contains(x))];

        return (company, newModules);
    }
    #endregion
}
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.CalculatePrice;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyInvoices.Create;

public sealed class CreateCompanyInvoice(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        ICalculatePriceModuleCompany calculatePriceModuleCompany
    ) : ICreateCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly ICalculatePriceModuleCompany _calculatePriceModuleCompany = calculatePriceModuleCompany;

    public async Task<CompanyInvoice?> Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules)
    {
        if (modules is null || modules.Length == 0)
        {
            return null;
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        ModuleEnum[] newModules = await CheckIfCompanyAlreadyHasModuleOrInvoice(companyId, modules);

        if (newModules is null || newModules.Length == 0)
        {
            return null;
        }

        (List<string> modulesStr, decimal finalPriceUsingProportionalPrice) = await CalculateInvoicePriceUsingProportionalPrice(userIdAuth, companyId, modules);

        CompanyInvoice invoice = new()
        {
            CompanyId = companyId,
            Modules = modules,
            Amount = finalPriceUsingProportionalPrice,
            Description = modules.Length > 1 ? $"Adição dos módulos: {string.Join("; ", modulesStr)}" : $"Adição do módulo: {string.Join("; ", modulesStr)}",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending
        };

        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }

    #region extras
    private async Task<ModuleEnum[]> CheckIfCompanyAlreadyHasModuleOrInvoice(Guid companyId, ModuleEnum[] modules)
    {
        List<ModuleEnum[]?> companyModules = await _context.Companies.AsNoTracking().Where(x => x.CompanyId == companyId && x.Status == true).Select(x => x.Modules).ToListAsync();

        if (companyModules is null || companyModules.Count == 0)
        {
            return [];
        }

        HashSet<ModuleEnum> existingCompanyModules = [.. companyModules.Where(x => x != null).SelectMany(x => x!)];
        ModuleEnum[] newModules = [.. modules.Where(x => !existingCompanyModules.Contains(x))];

        return newModules;
    }

    private async Task<(List<string> modulesStr, decimal finalPriceUsingProportionalPrice)> CalculateInvoicePriceUsingProportionalPrice(Guid userIdAuth, Guid companyId, ModuleEnum[] modules)
    {
        List<CalculatePriceModuleCompanyOutput> output = await _calculatePriceModuleCompany.Execute(userIdAuth, companyId, modules);

        List<string> modulesStr = [];
        decimal finalPriceUsingProportionalPrice = 0;

        foreach (var item in output)
        {
            modulesStr.Add(item.ModuleStr);
            finalPriceUsingProportionalPrice += item.ProportionalPrice;
        }

        return (modulesStr, finalPriceUsingProportionalPrice);
    }
    #endregion
}
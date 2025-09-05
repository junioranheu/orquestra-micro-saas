using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyInvoices.Create;

public sealed class CreateCompanyInvoice(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser
    ) : CompanyBase(context, checkIfUserIsLinkedCompanyUser), ICreateCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task<CompanyInvoice> Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules)
    {
        if (modules is null || modules.Length == 0)
        {
            throw new ArgumentException($"O parâmetro {nameof(modules)} está vazio.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        List<string> modulesStr = [];
        decimal amount = 0;

        foreach (var item in modules)
        {
            modulesStr.Add(GetEnumDesc(item));
            amount += ModuleHelper.GetPrice(item);
        }

        CompanyInvoice invoice = new()
        {
            CompanyId = companyId,
            Amount = amount,
            Description = modules.Length > 1 ? $"Adição dos módulos: {string.Join(", ", modulesStr)}" : $"Adição do módulo: {string.Join(", ", modulesStr)}",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending
        };

        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();

        return invoice;
    }
}
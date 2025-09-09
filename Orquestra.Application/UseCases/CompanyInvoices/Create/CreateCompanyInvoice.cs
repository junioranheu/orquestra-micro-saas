using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.CalculatePrice;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Email;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyInvoices.Create;

public sealed class CreateCompanyInvoice(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        ICalculatePriceModuleCompany calculatePriceModuleCompany,
        IEnvService env,
        IEmailService emailService
    ) : ICreateCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly ICalculatePriceModuleCompany _calculatePriceModuleCompany = calculatePriceModuleCompany;
    private readonly IEnvService _env = env;
    private readonly IEmailService _emailService = emailService;

    public async Task<CompanyInvoice?> Execute(Guid userIdAuth, Guid companyId, ModuleEnum[] modules, bool isCreateCompany = false)
    {
        if (modules is null || modules.Length == 0)
        {
            return null;
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        (Company? company, ModuleEnum[] newModules) = await CheckIfCompanyAlreadyHasModuleOrInvoice(companyId, modules, isCreateCompany);

        if (company is null || newModules is null || newModules.Length == 0)
        {
            return null;
        }

        (List<string> modulesStr, decimal finalPriceUsingProportionalPrice) = await CalculateInvoicePriceUsingProportionalPrice(userIdAuth, companyId, newModules);

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

        await SendEmail(company, invoice);

        return invoice;
    }

    #region extras
    private async Task<(Company? company, ModuleEnum[] newModules)> CheckIfCompanyAlreadyHasModuleOrInvoice(Guid companyId, ModuleEnum[] modules, bool isCreateCompany)
    {
        Company? company = await _context.Companies.AsNoTracking().Where(x => x.CompanyId == companyId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Company);

        if (isCreateCompany)
        {
            return (company, modules);
        }

        HashSet<ModuleEnum> existingCompanyModules = [.. company?.Modules ?? []];
        ModuleEnum[] newModules = [.. modules.Where(x => !existingCompanyModules.Contains(x))];

        return (company, newModules);
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

    private async Task SendEmail(Company company, CompanyInvoice invoice)
    {
        EnvOutput env = _env.GetUrls();
        string paymentUrl = $"{env.UrlBackend}/AEA";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.NameApp },
            { "[CompanyName]", company.Name },
            { "[InvoiceNumber]", invoice.CompanyInvoiceId.ToString() },
            { "[InvoiceDate]", GetDateDetails(withHour: false) },
            { "[ModuleDescription]", invoice.Description ?? string.Empty },
            { "[Price]", invoice.Amount.ToString() },
            { "[PaymentUrl]", paymentUrl }
        };

        string bodyHtml = _emailService.RenderTemplate("EmailCreateInvoice.html", values);
        await _emailService.SendEmail(to: company.Email, subject: $"Nova fatura — {company.Name} — {SystemConsts.NameApp}", body: bodyHtml);
    }
    #endregion
}
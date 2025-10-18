using Microsoft.EntityFrameworkCore;
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
        IEnvService env,
        IEmailService emailService
    ) : ICreateCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IEnvService _env = env;
    private readonly IEmailService _emailService = emailService;

    public async Task<CompanyInvoice?> Execute(Guid userIdAuth, Guid companyId, PlanTypeEnum planType, bool isCreateCompany = false)
    {
        if (planType == 0)
        {
            throw new ArgumentException($"O parâmetro {planType} está vazio.");
        }

        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        (decimal price, int _, string _, string[] _, int _) = PlanTypeHelper.GetValues(planType);

        Company company = await GetCompany(companyId);

        CompanyInvoice invoice = new()
        {
            CompanyId = companyId,
            PlanType = planType,
            Amount = price,
            Description = string.Empty,
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending
        };

        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();

        await SendEmail(company, invoice);

        return invoice;
    }

    #region extras
    private async Task<Company> GetCompany(Guid companyId)
    {
        Company company = await _context.Companies.AsNoTracking().Where(x => x.CompanyId == companyId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        return company;
    }

    private async Task SendEmail(Company company, CompanyInvoice invoice)
    {
        EnvOutput env = _env.GetUrls();
        string paymentUrl = $"{env.UrlBackend}/CompanyInvoice/Pay/{invoice.InvoiceNumber}";

        Dictionary<string, string> values = new()
        {
            { "[NameApp]", SystemConsts.App.NameApp },
            { "[CompanyName]", company.Name },
            { "[InvoiceNumber]", invoice.InvoiceNumber.ToString() },
            { "[InvoiceDate]", GetDateDetails(withHour: false) },
            { "[ModuleDescription]", invoice.Description ?? string.Empty },
            { "[Price]", invoice.Amount.ToString() },
            { "[PaymentUrl]", paymentUrl }
        };

        string bodyHtml = _emailService.RenderTemplate(SystemConsts.Templates.EmailCreateInvoice, values);
        await _emailService.SendEmail(to: company.Email, subject: $"Nova fatura — {company.Name} — {SystemConsts.App.NameApp}", body: bodyHtml);
    }
    #endregion
}
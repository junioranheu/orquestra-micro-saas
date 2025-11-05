using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Messaging.Publishers;
using Orquestra.Infrastructure.Services.Email.Models;
using Orquestra.Infrastructure.Services.Env;
using Orquestra.Infrastructure.Services.Env.Models;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyInvoices.Create;

public sealed class CreateCompanyInvoice(
        Context context,
        ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser,
        IEnvService env,
        IGenericPublisher publisher
    ) : ICreateCompanyInvoice
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;
    private readonly IEnvService _env = env;
    private readonly IGenericPublisher _publisher = publisher;

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
            Description = $"Plano {GetEnumDesc(planType).ToLowerInvariant()}",
            CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Pending
        };

        await _context.AddAsync(invoice);
        await _context.SaveChangesAsync();

        if (isCreateCompany && planType == PlanTypeEnum.Free)
        {
            invoice.CompanyInvoiceSituation = CompanyInvoiceSituationEnum.Paid;
            _context.Update(invoice);
            await _context.SaveChangesAsync();
        }

        if (planType != PlanTypeEnum.Free)
        {
            await SendEmail(company, invoice);
        }

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

        string bodyHtml = RenderTemplate(SystemConsts.Templates.EmailCreateInvoice, values);

        EmailInput input = new()
        {
            To = company.Email,
            Subject = $"Nova fatura — {company.Name} — {SystemConsts.App.NameApp}",
            Body = bodyHtml
        };

        await _publisher.Publish(SystemConsts.RabbitMQ.EmailQueue, input);
    }
    #endregion
}
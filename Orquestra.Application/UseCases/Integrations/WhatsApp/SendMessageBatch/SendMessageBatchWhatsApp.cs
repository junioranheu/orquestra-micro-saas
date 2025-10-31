using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;

public sealed class SendMessageBatchWhatsApp(IntegrationWhatsAppBaseDependencies deps) : IntegrationWhatsAppBase(), ISendMessageBatchWhatsApp
{
    private readonly Context _context = deps.Context;

    public async Task Execute(CancellationToken token)
    {
        List<Company> companies = await GetCompanies();
        List<Company> validCompanies = [];

        foreach (var item in companies)
        {
            bool isValid = Validate(company: item, mustThrow: false);

            if (!isValid)
            {
                continue;
            }

            validCompanies.Add(item);
        }

        if (validCompanies is null || validCompanies.Count == 0)
        {
            return;
        }

        List<IntegrationWhatsApp> integrations = await GetIntegrations(companies: validCompanies);

        await SendMessages(integrations);
    }

    #region extras
    private async Task<List<Company>> GetCompanies()
    {
        List<Company> companies = await _context.Companies.AsNoTracking().Where(x => x.Status == true).ToListAsync();

        return companies;
    }

    private async Task<List<IntegrationWhatsApp>> GetIntegrations(List<Company> companies)
    {
        List<Guid> companyIds = [.. companies.Select(x => x.CompanyId)];

        List<IntegrationWhatsApp> integrations = await _context.IntegrationsWhatsApp.
                                                 Include(x => x.Company)!.ThenInclude(x => x!.Clients)!.ThenInclude(x => x.Schedules).
                                                 AsNoTracking().
                                                 Where(x => companyIds.Contains(x.CompanyId) && x.Status == true).
                                                 ToListAsync();

        return integrations;
    }

    private async Task SendMessages(List<IntegrationWhatsApp> integrations)
    {

    }
    #endregion
}
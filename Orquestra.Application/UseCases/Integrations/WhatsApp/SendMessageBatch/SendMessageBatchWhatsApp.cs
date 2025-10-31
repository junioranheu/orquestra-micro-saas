using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;

public sealed class SendMessageBatchWhatsApp(IntegrationWhatsAppBaseDependencies deps) : IntegrationWhatsAppBase(), ISendMessageBatchWhatsApp
{
    private readonly Context _context = deps.Context;

    public async Task<int> Execute(CancellationToken token)
    {
        List<Company> companies = await GetCompanies();
        List<Company> validCompanies = [];

        foreach (var item in companies)
        {
            bool isValid = ValidateCompany(company: item, mustThrow: false);

            if (!isValid)
            {
                continue;
            }

            validCompanies.Add(item);
        }

        if (validCompanies is null || validCompanies.Count == 0)
        {
            return 0;
        }

        List<WhatsAppMessageBatchOutput> integrations = await GetIntegrations(companies: validCompanies);

        if (integrations is null || integrations.Count == 0)
        {
            return 0;
        }

        int amount = await SendMessages(integrations);

        return amount;
    }

    #region extras
    private async Task<List<Company>> GetCompanies()
    {
        List<Company> companies = await _context.Companies.AsNoTracking().Where(x => x.Status == true).ToListAsync();

        return companies;
    }

    private async Task<List<WhatsAppMessageBatchOutput>> GetIntegrations(List<Company> companies)
    {
        List<Guid> companyIds = [.. companies.Select(x => x.CompanyId)];

        List<WhatsAppMessageBatchOutput> outputs = await _context.IntegrationsWhatsApp.
            Include(x => x.Company).ThenInclude(x => x!.Clients)!.ThenInclude(x => x.Schedules).
            AsNoTracking().
            Where(x =>
                companyIds.Contains(x.CompanyId) &&
                x.Status == true &&
                x.Company!.Clients!.Any(c =>
                    c.Phone != "" &&
                    c.Schedules!.Any(s => s.ScheduleStatus != ScheduleStatusEnum.Completed && s.Status == true)     
                )
            ).
            SelectMany(integration => integration.Company!.Clients!.
                SelectMany(client => client.Schedules!.
                    Select(schedule => new WhatsAppMessageBatchOutput
                    {
                        CompanyName = integration.Company!.Name,
                        ClientName = client.FullName,
                        ClientPhone = client.Phone ?? string.Empty,
                        ScheduleDate = schedule.DateStart,
                        ScheduleStatus = schedule.ScheduleStatus,
                        MessageReminderBeforeSchedule = integration.MessageReminderBeforeSchedule,
                        MessageBeforeScheduleAlert = integration.MessageBeforeScheduleAlert,
                        MessageOnScheduleConfirmed = integration.MessageOnScheduleConfirmed,
                        MessageOnScheduleCanceled = integration.MessageOnScheduleCanceled
                    })
             )
            ).ToListAsync();

        return outputs;
    }

    private async Task<int> SendMessages(List<WhatsAppMessageBatchOutput> integrations)
    {
        int amount = integrations.Count;

        return amount;
    }
    #endregion
}
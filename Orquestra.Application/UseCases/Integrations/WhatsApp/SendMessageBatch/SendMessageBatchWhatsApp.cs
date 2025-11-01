using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.Infrastructure.Services.Sms;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.SendMessageBatch;

public sealed class SendMessageBatchWhatsApp(IntegrationWhatsAppBaseDependencies deps) : IntegrationWhatsAppBase(), ISendMessageBatchWhatsApp
{
    private readonly Context _context = deps.Context;
    private readonly ISmsService _smsService = deps.SmsService;

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

        await _smsService.SendSms(to: "982716339", from: SystemConsts.App.NameApp, text: "Teste");

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
        List<Guid> companyIds = companies?.Select(c => c.CompanyId).ToList() ?? [];

        if (companyIds.Count == 0)
        {
            return [];
        }

        var query =
            from s in _context.Schedules.AsNoTracking()
            join c in _context.Clients.AsNoTracking() on s.ClientId equals c.ClientId
            join co in _context.Companies.AsNoTracking() on s.CompanyId equals co.CompanyId
            join i in _context.IntegrationsWhatsApp.AsNoTracking() on co.CompanyId equals i.CompanyId
            where
                companyIds.Contains(co.CompanyId) &&    // Apenas as companies da lista validadas pelo ValidateCompany();
                c.Status == true &&                     // Empresa ativa;
                i.Status == true &&                     // Integração ativa;
                c.Status == true &&                     // Cliente ativo;
                !string.IsNullOrEmpty(c.Phone) &&       // Cliente com telefone;
                s.Status == true &&                     // Schedule ativo;
                s.ScheduleStatus != ScheduleStatusEnum.Completed // Schedule diferente de concluído;
            select new WhatsAppMessageBatchOutput
            {
                CompanyName = co.Name ?? string.Empty,
                ClientName = c.FullName,
                ClientPhone = c.Phone ?? string.Empty,
                ScheduleDate = s.DateStart,
                ScheduleStatus = s.ScheduleStatus,
                MessageReminderBeforeSchedule = i.MessageReminderBeforeSchedule ?? string.Empty,
                MessageBeforeScheduleAlert = i.MessageBeforeScheduleAlert ?? string.Empty,
                MessageOnScheduleConfirmed = i.MessageOnScheduleConfirmed ?? string.Empty,
                MessageOnScheduleCanceled = i.MessageOnScheduleCanceled ?? string.Empty
            };

        List<WhatsAppMessageBatchOutput> output = await query.ToListAsync();

        return output;
    }

    private async Task<int> SendMessages(List<WhatsAppMessageBatchOutput> integrations)
    {
        int amount = integrations.Count;

        return amount;
    }
    #endregion
}
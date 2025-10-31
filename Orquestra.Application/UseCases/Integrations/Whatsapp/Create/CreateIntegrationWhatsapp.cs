using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Integrations.WhatsApp.Base;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Integrations.WhatsApp.Create;

public sealed class CreateIntegrationWhatsApp(IntegrationWhatsAppBaseDependencies deps) : ICreateIntegrationWhatsApp
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid companyId, IntegrationWhatsApp? input = null)
    {
        bool isCreate = input is null;

        // Validar (linked company user);
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: companyId, userId: userIdAuth, needCompanyAdmin: true);

        // Validar plano e situação da empresa;
        if (!isCreate)
        {
            Company? company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyId == companyId);

            if (company?.PlanType == PlanTypeEnum.Free)
            {
                throw new InvalidOperationException($"O plano <b>{GetEnumDesc(PlanTypeEnum.Free).ToLowerInvariant()}</b> não tem acesso às integrações do WhatsApp.");
            }

            if (company?.CompanySituation == CompanySituationEnum.PendingPayment)
            {
                throw new InvalidOperationException($"Sua empresa está na situação de <b>{GetEnumDesc(CompanySituationEnum.PendingPayment).ToLowerInvariant()}</b>, portanto não pode prosseguir com esta ação.");
            }
        }

        // Deletar registros anteriores;
        if (!isCreate)
        {
            await DeletePrevious(companyId: companyId);
        }

        // Salvar;
        await Save(companyId, input);
    }

    #region extras
    private async Task DeletePrevious(Guid companyId)
    {
        List<IntegrationWhatsApp> rows = await _context.IntegrationsWhatsApp.
                                         // AsNoTracking(). // Propositalmente sem AsNoTracking;
                                         Where(x => x.CompanyId == companyId).
                                         ToListAsync();

        if (rows.Count == 0)
        {
            return;
        }

        _context.RemoveRange(rows);
    }

    private async Task Save(Guid companyId, IntegrationWhatsApp? input)
    {
        input ??= new IntegrationWhatsApp
        {
            CompanyId = companyId
        };

        await _context.AddAsync(input);
        await _context.SaveChangesAsync();
    }
    #endregion
}
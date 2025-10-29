using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Integrations.Whatsapp.Base;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Integrations.Whatsapp.Create;

public sealed class CreateIntegrationWhatsapp(IntegrationWhatsappBaseDependencies deps) : IntegrationWhatsappBase(deps), ICreateIntegrationWhatsapp
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid companyId, IntegrationWhatsapp? input = null)
    {
        // Validar;
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId: companyId, userId: userIdAuth, needCompanyAdmin: true);

        // Deletar;
        await DeletePrevious(companyId: companyId);

        // Salvar;
        await Save(companyId, input);
    }

    #region extras
    private async Task DeletePrevious(Guid companyId)
    {
        List<IntegrationWhatsapp> rows = await _context.IntegrationsWhatsapp.AsNoTracking().Where(x => x.CompanyId == companyId).ToListAsync();
        _context.RemoveRange(rows);
    }

    private async Task Save(Guid companyId, IntegrationWhatsapp? input)
    {
        input ??= new IntegrationWhatsapp
        {
            CompanyId = companyId
        };

        await _context.AddAsync(input);
        await _context.SaveChangesAsync();
    }
    #endregion
}
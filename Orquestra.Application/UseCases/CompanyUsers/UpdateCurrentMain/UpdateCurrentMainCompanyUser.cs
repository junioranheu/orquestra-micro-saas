using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.UpdateCurrentMain;

public sealed class UpdateCurrentMainCompanyUser(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : IUpdateCurrentMainCompanyUser
{
    private readonly Context _context = context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = checkIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: false);

        List<CompanyUser> listCompanyUser = await _context.CompanyUsers. // Propositalmente sem AsNoTracking;
                                            Where(x => x.UserId == userIdAuth).
                                            ToListAsync() ?? throw new Exception("Você não faz parte de nenhuma empresa.");

        CompanyUser companyUser = await _context.CompanyUsers. // Propositalmente sem AsNoTracking;
                                  Where(x => x.CompanyId == companyId && x.UserId == userIdAuth && x.Status == true).
                                  FirstOrDefaultAsync() ?? throw new Exception("Você não faz parte dessa empresa.");

        if (!companyUser.IsCurrentMainCompanyUser)
        {
            // Atualizar o IsCurrentMainCompanyUser em questão;
            companyUser.IsCurrentMainCompanyUser = true;

            _context.Update(companyUser);
            await _context.SaveChangesAsync();
        }

        // Certificar-se que todos os OUTROS IsCurrentMainCompanyUser estão como false;
        List<CompanyUser> restCompanies = [.. listCompanyUser.Where(x => x.IsCurrentMainCompanyUser == true).Except([companyUser])];

        if (restCompanies is null || restCompanies.Count == 0)
        {
            return;
        }

        restCompanies.ForEach(x => x.IsCurrentMainCompanyUser = false);

        _context.UpdateRange(restCompanies);
        await _context.SaveChangesAsync();
    }
}
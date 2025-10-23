using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.Companies.ResendVerifyEmail;

public sealed class ResendVerifyEmailCompany(CompanyBaseDependencies deps) : CompanyBase(deps), IResendVerifyEmailCompany
{
    private readonly Context _context = deps.Context;
    private readonly ICheckIfUserIsLinkedCompanyUser _checkIfUserIsLinkedCompanyUser = deps.CheckIfUserIsLinkedCompanyUser;

    public async Task Execute(Guid userIdAuth, Guid companyId)
    {
        await _checkIfUserIsLinkedCompanyUser.Execute(companyId, userId: userIdAuth, needCompanyAdmin: true);

        Company company = await Validate(companyId);
        UserOutput user = await GetUser(userIdAuth);

        await SendEmail(company, user);
    }

    #region extras
    private async Task<Company> Validate(Guid companyId)
    {
        var company = await _context.Companies.
                      AsNoTracking().
                      Where(x => x.CompanyId == companyId).
                      FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        if (company.Status)
        {
            throw new InvalidOperationException("Essa empresa já foi verificada anteriormente, portanto essa solicitação foi abortada.");
        }

        var verifications = await _context.Verifications.
                            // AsNoTracking(). // Propositalmente sem AsNoTracking;
                            Where(x => x.EntityId == companyId && x.VerificationType == VerificationTypeEnum.Company && x.Status == true).
                            ToListAsync();

        bool hasRecentVerification = verifications.Any(x => x.CreatedDate >= GetDate().AddDays(-1));

        if (hasRecentVerification)
        {
            throw new InvalidOperationException("Já existe uma solicitação de verificação feita nas últimas 24 horas. Tente novamente mais tarde.");
        }

        if (verifications is not null && verifications.Count != 0)
        {
            foreach (var item in verifications)
            {
                item.Status = false;
            }

            _context.UpdateRange(verifications);
            await _context.SaveChangesAsync();
        }

        return company;
    }

    private async Task<UserOutput> GetUser(Guid userIdAuth)
    {
        var user = await _context.Users.
                   AsNoTracking().
                   Where(x => x.UserId == userIdAuth).
                   FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warnings.NotFoundCompany);

        UserOutput output = new()
        {
            FullName = user.FullName,
            Email = user.Email
        };

        return output;
    }
    #endregion
}
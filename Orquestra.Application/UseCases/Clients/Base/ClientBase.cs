using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Base;

public partial class ClientBase(Context context, IGetCompanyUser getCompanyUser)
{
    private readonly Context _context = context;
    private readonly IGetCompanyUser _getCompanyUser = getCompanyUser;

    public async Task Validate(ClientInput input, Guid userId, bool isCreate)
    {
        List<CompanyUser>? companiesFromUser = await _getCompanyUser.Execute(companyId: Guid.Empty, userId: userId);
        bool? isAdmin = companiesFromUser?.Any(x => x.Users?.UserId == userId && x.CompanyId == input.CompanyId && (x.CompanyUserRole == CompanyUserRoleEnum.Administrator || x.CompanyUserRole == CompanyUserRoleEnum.Owner));

        if (input.CompanyId == Guid.Empty || companiesFromUser?.Count == 0 || !isAdmin.GetValueOrDefault())
        {
            throw new Exception("Apenas um administrador da empresa pode alterar suas informações");
        }

        bool anyCPF = await _context.Clients.AsNoTracking().AnyAsync(x => x.CPF == input.CPF && x.CompanyId == input.CompanyId && x.ClientId == input.ClientId);

        if (anyCPF)
        {
            throw new Exception("Este CPF já está registrado nesta empresa para outro cliente");
        }

        bool anyEmail = await _context.Clients.AsNoTracking().AnyAsync(x => x.Email == input.Email && x.CompanyId == input.CompanyId && x.Email == input.Email);

        if (anyEmail)
        {
            throw new Exception("Este e-mail já está registrado nesta empresa para outro cliente");
        }
    }
}
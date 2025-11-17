using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Companies.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using System;
using System.Data;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.Application.UseCases.CompanyUsers.GetCurrentMain;

public sealed class GetCurrentMainCompanyUser(Context context) : IGetCurrentMainCompanyUser
{
    private readonly Context _context = context;

    private static readonly Func<Context, Guid, Task<CompanyUser?>> _compiledGet =
        EF.CompileAsyncQuery((Context ctx, Guid uid) =>
            ctx.CompanyUsers.
            AsNoTracking().
            Include(x => x.Company).
            FirstOrDefault(x => x.UserId == uid && x.IsCurrentMainCompanyUser == true && x.Status == true));

    public async Task<(CompanyOutput? currentMainCompany, bool isUserAdm)> Execute(Guid userId)
    {
        CompanyUser? output = await _compiledGet(_context, userId);

        if (output is null || output?.Company is null)
        {
            return (null, false);
        }

        // Ignorar o campo CompanyUsers;
        TypeAdapterConfig config = new();
        config.NewConfig<Company, CompanyOutput>().Ignore(x => x.CompanyUsers!);

        CompanyOutput outputAdapt = output.Company.Adapt<CompanyOutput>(config);
        outputAdapt.UserModules = output.UserModules;

        if (outputAdapt is null)
        {
            return (null, false);
        }

        bool isUserAdm = output.CompanyUserRole == CompanyUserRoleEnum.Administrator;

        outputAdapt.IsAdm = isUserAdm;
        outputAdapt.IsOwner = output.InviterUserId is null || output.InviterUserId == Guid.Empty;
        outputAdapt.CompanySituationStr = GetEnumDesc(outputAdapt.CompanySituation);

        return (outputAdapt, isUserAdm);
    }
}
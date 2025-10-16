using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;

public sealed class GetCompanyUserByCompanyId(Context context) : IGetCompanyUserByCompanyId
{
    private readonly Context _context = context;

    public async Task<List<CompanyUserOutput>?> Execute(Guid companyId, Guid? userId = null)
    {
        var result = await _context.CompanyUsers.
                     Include(x => x.User).
                     Include(x => x.InviterUser).
                     AsNoTracking().
                     Where(x =>
                        (companyId == Guid.Empty || x.CompanyId == companyId) &&
                        ((userId == Guid.Empty || userId == null) || x.UserId == userId)
                        // x.Status == true // Essa query NÃO deve buscar por Status true, porque senão pode cagar tudo;
                     ).
                     GroupBy(x => new { x.CompanyId, x.UserId, x.CompanyUserRole }).
                     Select(g => g.FirstOrDefault()).
                     ToListAsync();

        if (result is null || result.Count == 0)
        {
            return [];
        }

        var output = result.Adapt<List<CompanyUserOutput>>();
        NormalizeOutput(output);

        return output;
    }

    public async Task<(IEnumerable<CompanyUserOutput> output, int count)> Execute(PaginationInput pagination, CompanyUserFilterInput input, Guid userIdAuth, Guid companyId)
    {
        bool hasPermission = await _context.CompanyUsers.
                             AsNoTracking().
                             Where(x =>
                                x.CompanyId == companyId &&
                                x.UserId == userIdAuth &&
                                x.Status == true
                             ).AnyAsync();

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException(SystemConsts.Warnings.InvalidLinkedCompanyUser);
        }

        #region convert
        CompanyUserRoleEnum? companyUserRoleNormalized = null;
        ModuleEnum[] moduleNormalized = [];

        if (!string.IsNullOrEmpty(input.CompanyUserRole))
        {
            if (Enum.TryParse(input.CompanyUserRole, ignoreCase: true, out CompanyUserRoleEnum parsedRole))
            {
                companyUserRoleNormalized = parsedRole;
            }
        }

        if (input.UserModules is { Length: > 0 })
        {
            moduleNormalized = [.. input.UserModules.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => Enum.TryParse(x, out ModuleEnum parsed) ? parsed : (ModuleEnum?)null).Where(x => x.HasValue).Select(x => x!.Value)];
        }
        #endregion

        var query = _context.CompanyUsers.
                    Include(x => x.User).
                    AsNoTracking().
                    Where(x =>
                        x.CompanyId == companyId &&
                        x.Status == true &&
                        (companyUserRoleNormalized == null || x.CompanyUserRole == companyUserRoleNormalized) &&
                        ((moduleNormalized == null || !moduleNormalized.Any()) || x.UserModules!.Intersect(moduleNormalized).Any()) &&
                        (string.IsNullOrEmpty(input.FullName) || x.User!.FullName.ToLower().Contains(input.FullName.ToLower())) &&
                        (string.IsNullOrEmpty(input.Email) || x.User!.Email!.ToLower().Contains(input.Email.ToLower()))
                    ).OrderBy(x => x.User!.FullName);

        (IEnumerable<CompanyUser> linq, int count) = await PagedQuery.Execute(query, pagination);
        var output = linq.Adapt<List<CompanyUserOutput>>();

        NormalizeOutput(output);

        return (output, count);
    }

    #region extras
    private static void NormalizeOutput(List<CompanyUserOutput> output)
    {
        foreach (var item in output)
        {
            if (item is null)
            {
                continue;
            }

            item.IsOwner = item?.InviterUserId is null;
        }
    }
    #endregion
}
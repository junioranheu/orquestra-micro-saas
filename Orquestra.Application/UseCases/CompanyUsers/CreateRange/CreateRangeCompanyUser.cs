using Mapster;
using Orquestra.Application.UseCases.CompanyUsers.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public sealed class CreateRangeCompanyUser(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : CompanyUserBase(context, checkIfUserIsLinkedCompanyUser), ICreateRangeCompanyUser
{
    private readonly Context _context = context;

    public async Task<List<CompanyUserOutput>> Execute(Guid userId, List<CompanyUserInput> input)
    {
        foreach (var item in input)
        {
            await Validate(input: item, userId, isCreate: true);
        }

        var companyUsers = input.Adapt<List<CompanyUser>>();

        NormalizeOwnerProps(companyUsers);

        await _context.AddRangeAsync(companyUsers);
        await _context.SaveChangesAsync();

        var output = companyUsers.Adapt<List<CompanyUserOutput>>();

        return output;
    }

    #region extras
    private static void NormalizeOwnerProps(List<CompanyUser> companyUsers)
    {
        if (companyUsers.Count != 1)
        {
            return;
        }

        CompanyUser? first = companyUsers.FirstOrDefault();

        if (first?.CompanyUserRole == CompanyUserRoleEnum.Owner)
        {
            first.IsAccountVerified = true;
            first.IsCurrentMainCompanyUser = true;
        }
    }
    #endregion
}
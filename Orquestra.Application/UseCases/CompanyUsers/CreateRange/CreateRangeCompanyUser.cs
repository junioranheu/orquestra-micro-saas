using Mapster;
using Orquestra.Application.UseCases.CompanyUsers.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public sealed class CreateRangeCompanyUser(Context context,ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : CompanyUserBase(context, checkIfUserIsLinkedCompanyUser), ICreateRangeCompanyUser
{
    private readonly Context _context = context;

    public async Task Execute(Guid userId, List<CompanyUserInput> input)
    {
        foreach (var item in input)
        {
            await Validate(input: item, userId, isCreate: true);
        }

        var companyUsers = input.Adapt<List<CompanyUser>>();

        await _context.AddRangeAsync(companyUsers);
        await _context.SaveChangesAsync();
    }
}
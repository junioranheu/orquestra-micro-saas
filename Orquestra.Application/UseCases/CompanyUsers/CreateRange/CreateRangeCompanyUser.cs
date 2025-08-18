using AutoMapper;
using Orquestra.Application.UseCases.CompanyUsers.Base;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public sealed class CreateRangeCompanyUser(Context context, IMapper map, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : CompanyUserBase(context, checkIfUserIsLinkedCompanyUser), ICreateRangeCompanyUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task Execute(Guid userId, List<CompanyUserInput> input)
    {
        foreach (var item in input)
        {
            await Validate(input: item, userId, isCreate: true);
        }

        var companyUsers = _map.Map<List<CompanyUser>>(input);

        await _context.AddRangeAsync(companyUsers);
        await _context.SaveChangesAsync();
    }
}
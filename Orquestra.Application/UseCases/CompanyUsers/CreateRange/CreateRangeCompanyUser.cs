using AutoMapper;
using Orquestra.Application.UseCases.CompanyUsers.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.CompanyUsers.CreateRange;

public sealed class CreateRangeCompanyUser(Context context, IMapper map) : ICreateRangeCompanyUser
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task Execute(List<CompanyUserInput> input)
    {
        var companyUsers = _map.Map<List<CompanyUser>>(input);

        await _context.AddRangeAsync(companyUsers);
        await _context.SaveChangesAsync();
    }
}
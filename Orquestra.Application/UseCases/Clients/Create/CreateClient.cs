using AutoMapper;
using Orquestra.Application.UseCases.Clients.Base;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Create;

public sealed class CreateClient(Context context, IMapper map, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ClientBase(context, checkIfUserIsLinkedCompanyUser), ICreateClient
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task Execute(Guid userId, ClientInput input)
    {
        await Validate(input, userId, isCreate: true);
        var client = _map.Map<Client>(input);

        await _context.AddAsync(client);
        await _context.SaveChangesAsync();
    }
}
using Mapster;
using Orquestra.Application.UseCases.Clients.Base;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Create;

public sealed class CreateClient(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ClientBase(context, checkIfUserIsLinkedCompanyUser), ICreateClient
{
    private readonly Context _context = context;

    public async Task<Guid> Execute(Guid userIdAuth, ClientInput input)
    {
        await Validate(input, userIdAuth, isCreate: true);
        var client = input.Adapt<Client>();

        await _context.AddAsync(client);
        await _context.SaveChangesAsync();

        return client.ClientId;
    }
}
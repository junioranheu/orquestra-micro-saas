using AutoMapper;
using Orquestra.Application.UseCases.Clients.Base;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.Get;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Create;

public sealed class CreateClient(Context context, IMapper map, IGetCompanyUser getCompanyUser) : ClientBase(context, getCompanyUser), ICreateClient
{
    private readonly Context _context = context;
    private readonly IMapper _map = map;

    public async Task<ClientOutput> Execute(Guid userId, ClientInput input)
    {
        await Validate(input, userId, isCreate: true);
        Client client = await Save(input);

        ClientOutput? output = _map.Map<ClientOutput>(client);

        return output;
    }

    #region extras
    private async Task<Client> Save(ClientInput input)
    {
        Client client = _map.Map<Client>(input);

        await _context.AddAsync(client);
        await _context.SaveChangesAsync();

        return client;
    }
    #endregion
}
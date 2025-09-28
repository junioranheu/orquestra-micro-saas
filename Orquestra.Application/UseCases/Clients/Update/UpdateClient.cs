using Mapster;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Clients.Base;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;

namespace Orquestra.Application.UseCases.Clients.Update;

public sealed class UpdateClient(Context context, ICheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser) : ClientBase(context, checkIfUserIsLinkedCompanyUser), IUpdateClient
{
    private readonly Context _context = context;

    public async Task<ClientOutput> Execute(Guid userIdAuth, ClientInput input)
    {
        await Validate(input, userIdAuth, isCreate: false);
        Client client = await Update(input);

        var output = client.Adapt<ClientOutput>();

        return output;
    }

    #region extras
    private async Task<Client> Update(ClientInput input)
    {
        Client? client = await _context.Clients.AsNoTracking().Where(x => x.ClientId == input.ClientId).FirstOrDefaultAsync() ?? throw new KeyNotFoundException(SystemConsts.Warn_NotFound_Client);

        client.FullName = input.FullName ?? client.FullName;
        client.Email = input.Email ?? client.Email;
        client.CPF = input.CPF ?? client.CPF;
        client.Address = input.Address ?? client.Address;
        client.DateOfBirth = input.DateOfBirth > DateTime.MinValue ? input.DateOfBirth : client.DateOfBirth;
        client.Phone = input.Phone ?? client.Phone;
        client.Notes = input.Notes ?? client.Notes;

        _context.Update(client);
        await _context.SaveChangesAsync();

        return client;
    }
    #endregion
}
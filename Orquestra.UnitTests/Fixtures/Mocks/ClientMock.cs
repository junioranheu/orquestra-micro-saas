using AutoMapper;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Domain.Entities;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.UnitTests.Fixtures.Mocks;

public static class ClientMock
{
    public static ClientInput Create()
    {
        var input = new ClientInput
        {
            ClientId = Guid.NewGuid(),
            FullName = GetRandomString(GetRandomNumber(5, 15), false)
        };

        return input;
    }

    public static List<ClientInput> CreateList(int j)
    {
        List<ClientInput> list = [];

        for (int i = 0; i < j; i++)
        {
            list.Add(Create());
        }

        return list;
    }

    public static Client Create(IMapper _map)
    {
        Client output = _map.Map<Client>(Create());
        return output;
    }

    public static List<Client> CreateList(IMapper _map, int j)
    {
        List<Client> output = _map.Map<List<Client>>(CreateList(j));
        return output;
    }
}
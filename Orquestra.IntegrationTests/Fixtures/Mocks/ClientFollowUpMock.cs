using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class ClientFollowUpMock
{
    public static ClientFollowUp Create(Client? client = null)
    {
        client ??= ClientMock.Create();

        ClientFollowUp input = new()
        {
            ClientFollowUpId = Guid.NewGuid(),
            ClientId = client.ClientId,
            Client = client,
            Observation = "Cliente demonstrou interesse em retornar na próxima semana para revisão do tratamento.",
            ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress,
            Images =
            [
                // Simula imagens em binário;
                [0xFF, 0xD8, 0xFF, 0xE0],
                [0x89, 0x50, 0x4E, 0x47]
            ],
            ImagesContentType =
            [
                "image/jpeg",
                "image/png"
            ]
        };

        return input;
    }

    public static List<ClientFollowUp> CreateList(int amount)
    {
        List<ClientFollowUp> list = [];

        for (int i = 0; i < amount; i++)
        {
            list.Add(Create());
        }

        return list;
    }
}
using Orquestra.Domain.Entities;

namespace Orquestra.IntegrationTests.Fixtures.Mocks;

public static class LocationMock
{
    public static List<LocationState> CreateStatesFixture(bool includeInactive = false)
    {
        List<LocationState> list =
        [
            new LocationState
            {
                Name = "São Paulo",
                Status = true
            },
            new LocationState
            {
                Name = "Rio de Janeiro",
                Status = true
            }
        ];

        if (includeInactive)
        {
            list.Add(new LocationState
            {
                Name = "Estado do Kross",
                Status = false
            });
        }

        return list;
    }

    public static List<LocationCity> CreateCitiesFixture(bool includeInactive = false)
    {
        LocationState state = new()
        {
            Name = "São Paulo",
            Status = true
        };

        List<LocationCity> list =
        [
                new()
                {
                    Name = "Lorena",
                    Status = true,
                    LocationState = state
                },
                new()
                {
                    Name = "São Paulo",
                    Status = true,
                    LocationState = state
                }
            ];

        if (includeInactive)
        {
            list.Add(new()
            {
                Name = "Cidade do Kross",
                Status = false,
                LocationState = state
            });
        }

        return list;
    }
}
using Orquestra.Application.UseCases.Locations.Cities.Get;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Locations;

public sealed class GetCityTests
{
    [Fact]
    public async Task Execute_ShouldReturnOnlyActiveCities_WhenNoStateIdProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        List<LocationCity> cities = LocationMock.CreateCitiesFixture(includeInactive: true);

        await context.LocationCities.AddRangeAsync(cities);
        await context.SaveChangesAsync();

        GetCity sut = CreateSut(context);

        // Act;
        List<LocationCity>? result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, x => Assert.True(x.Status));
        Assert.All(result, x => Assert.NotNull(x.LocationState));
        Assert.DoesNotContain(result, x => x.Name == "Cidade do Kross");
    }

    [Fact]
    public async Task Execute_ShouldReturnAllCities_WhenStateIdIsInvalidOrNegative()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        List<LocationCity> cities = LocationMock.CreateCitiesFixture(includeInactive: false);

        await context.LocationCities.AddRangeAsync(cities);
        await context.SaveChangesAsync();

        GetCity sut = CreateSut(context);

        // Act;
        List<LocationCity>? resultNegative = await sut.Execute(-1);
        List<LocationCity>? resultZero = await sut.Execute(0);

        // Assert;
        Assert.Equal(cities.Count, resultNegative?.Count);
        Assert.Equal(cities.Count, resultZero?.Count);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoActiveCitiesExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        List<LocationCity> cities = LocationMock.CreateCitiesFixture(includeInactive: true);
        cities.ForEach(x => x.Status = false);

        await context.LocationCities.AddRangeAsync(cities);
        await context.SaveChangesAsync();

        GetCity sut = CreateSut(context);

        // Act;
        List<LocationCity>? result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetCity sut = CreateSut(context);

        // Act;
        List<LocationCity>? result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnOnlyCitiesFromGivenLocationStateId()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        // Cria dois estados diferentes;
        LocationState stateA = new() { Name = "São Paulo", Status = true };
        LocationState stateB = new() { Name = "Minas Gerais", Status = true };

        // Cria cidades em estados diferentes;
        List<LocationCity> cities =
        [
            new() { Name = "Lorena", Status = true, LocationState = stateA },
            new() { Name = "São Paulo", Status = true, LocationState = stateA },
            new() { Name = "Belo Horizonte", Status = true, LocationState = stateB }
        ];

        await context.LocationCities.AddRangeAsync(cities);
        await context.SaveChangesAsync();

        GetCity sut = CreateSut(context);

        // Act;
        List<LocationCity>? result = await sut.Execute(stateA.LocationStateId);

        // Assert;
        Assert.NotNull(result);
        Assert.All(result, x => Assert.Equal(stateA.LocationStateId, x.LocationStateId));
        Assert.Equal(2, result.Count); // Só as 2 do estado A
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenLocationStateIdDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        List<LocationCity> cities = LocationMock.CreateCitiesFixture(includeInactive: false);

        await context.LocationCities.AddRangeAsync(cities);
        await context.SaveChangesAsync();

        GetCity sut = CreateSut(context);

        int nonExistingId = 9999;

        // Act;
        List<LocationCity>? result = await sut.Execute(nonExistingId);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #region extras
    private static GetCity CreateSut(Context context)
    {
        GetCity getCity = new(context);

        return getCity;
    }
    #endregion
}
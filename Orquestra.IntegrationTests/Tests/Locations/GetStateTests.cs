using Orquestra.Application.UseCases.Locations.States.Get;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Locations;

public sealed class GetStateTests
{
    [Fact]
    public async Task Execute_ShouldReturnOnlyActiveStates()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        List<LocationState> states = LocationMock.CreateStatesFixture(includeInactive: true);

        await context.LocationStates.AddRangeAsync(states);
        await context.SaveChangesAsync();

        GetState sut = CreateSut(context);

        // Act;
        var result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, x => Assert.True(x.Status));
        Assert.DoesNotContain(result, x => x.Name == "Estado do Kross");
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenNoActiveStatesExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        List<LocationState> states = LocationMock.CreateStatesFixture(includeInactive: true);
        states.ForEach(x => x.Status = false);

        await context.LocationStates.AddRangeAsync(states);
        await context.SaveChangesAsync();

        GetState sut = CreateSut(context);

        // Act;
        List<LocationState>? result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenDatabaseIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetState sut = CreateSut(context);

        // Act;
        var result = await sut.Execute();

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #region extras
    private static GetState CreateSut(Context context)
    {
        GetState getState = new(context);

        return getState;
    }
    #endregion
}
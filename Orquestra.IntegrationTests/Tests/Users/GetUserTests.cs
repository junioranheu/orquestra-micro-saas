using Orquestra.Application.UseCases.Shared;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class GetUserTests
{
    [Fact]
    public async Task Execute_ByInput_ShouldReturnUser_WhenValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetUser sut = CreateSut(context);

        UserInput input = new() { Email = user.Email };

        // Act;
        (UserOutput? output, string password) = await sut.Execute(input);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(user.UserId, output.UserId);
        Assert.Equal(user.Password, password);
    }

    [Fact]
    public async Task Execute_ByInput_ShouldThrow_WhenAllParamsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetUser sut = CreateSut(context);

        UserInput input = new();

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ByInput_ShouldThrow_WhenUserInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        User user = UserMock.Create();

        await Fixture.Save(context, user);

        user.Status = false;
        context.Update(user);
        await context.SaveChangesAsync();

        GetUser sut = CreateSut(context);
        UserInput input = new() { UserId = user.UserId };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(input));
    }

    [Fact]
    public async Task Execute_ByInput_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetUser sut = CreateSut(context);

        UserInput input = new() { Email = "notfound@example.com" };

        // Act;
        (UserOutput? output, string password) = await sut.Execute(input);

        // Assert;
        Assert.Null(output);
        Assert.Equal(string.Empty, password);
    }

    [Fact]
    public async Task Execute_ById_ShouldReturnUser_WhenValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetUser sut = CreateSut(context);

        // Act;
        (UserOutput output, _) = await sut.Execute(user.UserId);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(user.UserId, output.UserId);
    }

    [Fact]
    public async Task Execute_ById_ShouldReturnEmptyOutput_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetUser sut = CreateSut(context);

        // Act;
        (UserOutput output, _) = await sut.Execute(Guid.NewGuid(), throwIfStatusFalse: false);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(Guid.Empty, output.UserId);
    }

    [Fact]
    public async Task Execute_ById_ShouldThrow_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        GetUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(Guid.NewGuid(), throwIfStatusFalse: true));
    }

    [Fact]
    public async Task Execute_ById_ShouldThrow_WhenInactiveAndThrowIfStatusFalse_IsTrue()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        user.Status = false;
        context.Update(user);
        await context.SaveChangesAsync();

        GetUser sut = CreateSut(context);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, throwIfStatusFalse: true));
    }

    [Fact]
    public async Task Execute_ById_ShouldReturnUser_WhenInactiveAndThrowIfStatusFalse_IsFalse()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        user.Status = false;
        context.Update(user);
        await context.SaveChangesAsync();

        GetUser sut = CreateSut(context);

        // Act;
        (UserOutput output, _) = await sut.Execute(user.UserId, throwIfStatusFalse: false);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(user.UserId, output.UserId);
    }

    [Fact]
    public async Task Execute_Pagination_ShouldReturnPagedUsers()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        List<User> users = UserMock.CreateList(10);

        foreach (var item in users)
        {
            await Fixture.Save(context, item);
        }

        GetUser sut = CreateSut(context);

        PaginationInput pagination = new() { Index = 1, Limit = 5 };

        // Act;
        (IEnumerable<UserOutput> output, int count) = await sut.Execute(pagination);

        // Assert;
        Assert.Equal(10, count);
        Assert.Equal(5, output.Count());
    }

    #region helpers
    private static GetUser CreateSut(Context context)
    {
        GetUser getUser = new(context);

        return getUser;
    }
    #endregion
}
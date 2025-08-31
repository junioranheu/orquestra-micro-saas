using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Application.UseCases.Users.Update;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Encrypt;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class UpdateUserTests
{
    [Fact]
    public async Task Execute_ShouldUpdateFullNameAndEmail_WhenValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create("Old Name", "old@example.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        UpdateUser sut = CreateSut(context);

        UserInput input = new()
        {
            FullName = "New Name",
            Email = "new@example.com",
            Password = "Toquinho22@"
        };

        // Act;
        var output = await sut.Execute(user.UserId, input);

        // Assert;
        Assert.Equal("New Name", output.FullName);
        Assert.Equal("new@example.com", output.Email);
    }

    [Fact]
    public async Task Execute_ShouldNotChangePassword_WhenPasswordIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create("Name", "test@example.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        UpdateUser sut = CreateSut(context);

        UserInput input = new()
        {
            FullName = "Other Name",
            Password = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldEncryptAndChangePassword_WhenProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create("Pebassauro Réx", "pass@example.com", UserRoleEnum.Administrator);
        string oldPassword = user.Password;
        await Fixture.Save(context, user);

        UpdateUser sut = CreateSut(context);

        string newPassword = "newPassword123!";

        UserInput input = new()
        {
            FullName = user.FullName,
            Email = user.Email,
            Password = newPassword
        };

        // Act;
        UserOutput output = await sut.Execute(user.UserId, input);

        // Assert;
        User updated = await context.Users.AsNoTracking().FirstAsync(x => x.UserId == user.UserId);

        Assert.NotEqual(oldPassword, updated.Password);
        Assert.NotEqual(EncryptPassword(newPassword), updated.Password); 
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UpdateUser sut = CreateSut(context);

        UserInput input = new() { FullName = "Test" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(Guid.NewGuid(), input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserInactive()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create("Inactive", "inactive@example.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        user.Status = false;
        context.Update(user);
        await context.SaveChangesAsync();

        UpdateUser sut = CreateSut(context);

        UserInput input = new () { FullName = "Inactive Update" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentException_WhenInputInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create("Name", "test2@example.com", UserRoleEnum.Administrator);
        await Fixture.Save(context, user);

        UpdateUser sut = CreateSut(context);

        UserInput input = new(); // Invalid, missing fields;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    #region helpers
    private static UpdateUser CreateSut(Context context)
    {
        GetUser getUser = new(context);
        UpdateUser updateUser = new(context, getUser);

        return updateUser;
    }
    #endregion
}
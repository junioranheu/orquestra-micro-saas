using Orquestra.Application.UseCases.Users.Base;
using Orquestra.Application.UseCases.Users.Get;
using Orquestra.Application.UseCases.Users.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Users;

public sealed class UserBaseTests
{
    [Fact]
    public async Task Validate_ShouldThrow_WhenEmailIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UserBase sut = CreateSut(context);

        UserInput input = new() { FullName = "Junior Test", Email = "email-invalido", Password = "Senha123@" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, Guid.NewGuid(), isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenEmailAlreadyExistsOnCreate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User existing = new()
        {
            FullName = "Usuario Existente",
            Email = "duplicado@teste.com",
            Password = "Senha123!",
            Role = UserRoleEnum.Common,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity,
            RecoverPasswordAnswer = GetRandomString(10),
            Status = true
        };

        await context.Users.AddAsync(existing);
        await context.SaveChangesAsync();

        UserBase sut = CreateSut(context);

        UserInput input = new()
        {
            FullName = "Outro Usuario",
            Email = existing.Email,
            Password = "SenhaNova123!"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, Guid.NewGuid(), isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUnauthorizedOnUpdate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User existing = new()
        {
            FullName = "Usuario Atual",
            Email = "atual@teste.com",
            Password = "Senha123!",
            Role = UserRoleEnum.Common,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity,
            RecoverPasswordAnswer = GetRandomString(10),
            Status = true
        };

        await context.Users.AddAsync(existing);
        await context.SaveChangesAsync();

        UserBase sut = CreateSut(context);

        UserInput input = new()
        {
            FullName = "Novo Nome",
            Email = existing.Email,
            Password = "SenhaNova123!"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Validate(input, Guid.NewGuid(), isCreate: false));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenFullNameIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UserBase sut = CreateSut(context);

        UserInput input = new() { FullName = "A", Email = "junior@teste.com", Password = "Senha123@" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, Guid.NewGuid(), isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenPasswordIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UserBase sut = CreateSut(context);

        UserInput input = new() { FullName = "Junior Test", Email = "junior@teste.com", Password = "123" };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, Guid.NewGuid(), isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid_OnCreate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();
        UserBase sut = CreateSut(context);

        UserInput input = new()
        {
            FullName = "Junior Test",
            Email = "junior@teste.com",
            Password = "Senha123@"
        };

        // Act;
        await sut.Validate(input, Guid.NewGuid(), isCreate: true);

        // Assert;
        Assert.Equal("junior@teste.com", input.Email);
        Assert.Equal("Junior Test", input.FullName);
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid_OnUpdate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User existing = new()
        {
            FullName = "Usuario Atual",
            Email = "atual@teste.com",
            Password = "Senha123!",
            Role = UserRoleEnum.Common,
            RecoverPasswordQuestion = RecoverPasswordQuestionEnum.BirthCity,
            RecoverPasswordAnswer = GetRandomString(10),
            Status = true
        };

        await context.Users.AddAsync(existing);
        await context.SaveChangesAsync();

        UserBase sut = CreateSut(context);

        UserInput input = new()
        {
            FullName = "Nome Atualizado",
            Email = "atual@teste.com",
            Password = "Senha123!"
        };

        // Act;
        await sut.Validate(input, existing.UserId, isCreate: false);

        // Assert;
        Assert.Equal("atual@teste.com", input.Email);
        Assert.Equal("Nome Atualizado", input.FullName);
    }

    #region helpers
    private static UserBase CreateSut(Context context)
    {
        GetUser getUser = new(context);

        UserBase userBase = new(getUser);

        return userBase;
    }
    #endregion
}
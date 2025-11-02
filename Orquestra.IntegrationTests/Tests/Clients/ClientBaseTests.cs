using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.Base;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Clients;

public sealed class ClientBaseTests
{
    [Fact]
    public async Task Validate_ShouldThrow_WhenFullNameIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "A",
            Email = "cliente@teste.com",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = Guid.NewGuid()
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenEmailIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Test",
            Email = "email-invalido",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = Guid.NewGuid()
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCPFIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Test",
            Email = "cliente@teste.com",
            CPF = "123",
            Phone = "11999999999",
            CompanyId = Guid.NewGuid()
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenPhoneIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Test",
            Email = "cliente@teste.com",
            CPF = "12345678901",
            Phone = "abc",
            CompanyId = Guid.NewGuid()
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenCPFAlreadyExistsOnCreate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();

        Client existing = new()
        {
            FullName = "Cliente Existente",
            Email = "existente@teste.com",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = companyId,
            Status = true
        };

        await context.Clients.AddAsync(existing);
        await context.SaveChangesAsync();

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Novo",
            Email = "novo@teste.com",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = companyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenEmailAlreadyExistsOnCreate()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Guid companyId = Guid.NewGuid();

        Client existing = new()
        {
            FullName = "Cliente Existente",
            Email = "duplicado@teste.com",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = companyId,
            Status = true
        };

        await context.Clients.AddAsync(existing);
        await context.SaveChangesAsync();

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Novo",
            Email = "duplicado@teste.com",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = companyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid_WithEmailAndPhone()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Test",
            Email = "cliente@teste.com",
            CPF = "44571955880",
            Phone = "12982716322",
            CompanyId = Guid.NewGuid()
        };

        // Act;
        await sut.Validate(input, user.UserId, isCreate: true);

        // Assert;
        Assert.Equal("cliente@teste.com", input.Email);
        Assert.Equal("Cliente Test", input.FullName);
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid_WithoutEmailAndPhone()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        ClientBase sut = CreateSut(context, user);

        ClientInput input = new()
        {
            FullName = "Cliente Test",
            Email = string.Empty,
            CPF = "44571955880",
            Phone = string.Empty,
            CompanyId = Guid.NewGuid()
        };

        // Act;
        await sut.Validate(input, user.UserId, isCreate: true);

        // Assert;
        Assert.Equal(string.Empty, input.Email);
        Assert.Equal("Cliente Test", input.FullName);
    }

    #region helpers
    private static ClientBase CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        ClientBase clientBase = new(context, checkIfUserIsLinkedCompanyUser);

        return clientBase;
    }
    #endregion
}
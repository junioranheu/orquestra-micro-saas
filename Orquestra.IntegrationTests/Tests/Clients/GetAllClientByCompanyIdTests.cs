using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.Clients.GetAllByCompanyId;
using Orquestra.Application.UseCases.Clients.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Clients;

public sealed class GetAllClientByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnClients_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client1 = ClientMock.Create();
        await Fixture.Save(context, client1);

        client1.CompanyId = company.CompanyId;
        context.Update(client1);
        await context.SaveChangesAsync();

        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);

        client2.CompanyId = company.CompanyId;
        context.Update(client2);
        await context.SaveChangesAsync();

       CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetAllClientByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ClientOutput>? result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.ClientId == client1.ClientId);
        Assert.Contains(result, r => r.ClientId == client2.ClientId);
    }

    [Fact]
    public async Task Execute_ShouldReturnPaginatedClients_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client1 = ClientMock.Create();
        await Fixture.Save(context, client1);

        client1.CompanyId = company.CompanyId;
        client1.Status = true;
        client1.FullName = "Mariana Scalzaretto";
        client1.Email = "mariana@example.com";
        context.Update(client1);
        await context.SaveChangesAsync();

        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);

        client2.CompanyId = company.CompanyId;
        client2.Status = true;
        client2.FullName = "Pedro Sempu";
        client2.Email = "pedro@example.com";
        context.Update(client2);
        await context.SaveChangesAsync();

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new()
        {
            Index = 0,
            Limit = 10
        };

        ClientInput input = new()
        {
            FullName = string.Empty,
            Email = string.Empty,
            CPF = string.Empty,
            Address = string.Empty,
            DateOfBirth = null,
            Phone = string.Empty,
            Notes = string.Empty
        };

        var sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(2, count);
        Assert.Equal(2, output.Count());
        Assert.Contains(output, r => r.ClientId == client1.ClientId);
        Assert.Contains(output, r => r.ClientId == client2.ClientId);
    }

    [Fact]
    public async Task Execute_ShouldReturnFilteredClients_WhenFullNameMatches()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client1 = ClientMock.Create();
        await Fixture.Save(context, client1);
        client1.CompanyId = company.CompanyId;
        client1.FullName = "Mariana Scalzaretto";
        client1.Status = true;
        context.Update(client1);

        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);
        client2.CompanyId = company.CompanyId;
        client2.FullName = "Pedro Sempu";
        client2.Status = true;
        context.Update(client2);

        await context.SaveChangesAsync();

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };
        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientInput input = new() { FullName = "Mariana" };

        var sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Single(output);
        Assert.Equal(1, count);
        Assert.Equal(client1.ClientId, output.First().ClientId);
    }

    [Fact]
    public async Task Execute_ShouldReturnFilteredClients_WhenDateOfBirthMatches()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client1 = ClientMock.Create();
        await Fixture.Save(context, client1);
        client1.CompanyId = company.CompanyId;
        client1.DateOfBirth = new DateTime(1997, 03, 15);
        client1.Status = true;
        context.Update(client1);

        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);
        client2.CompanyId = company.CompanyId;
        client2.DateOfBirth = new DateTime(2000, 08, 25);
        client2.Status = true;
        context.Update(client2);

        await context.SaveChangesAsync();

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };
        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientInput input = new() { DateOfBirth = new DateTime(1997, 03, 15) };

        var sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Single(output);
        Assert.Equal(1, count);
        Assert.Equal(client1.ClientId, output.First().ClientId);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmptyList_WhenCompanyHasNoClients()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetAllClientByCompanyId sut = CreateSut(context, user);

        // Act;
        List<ClientOutput>? result = await sut.Execute(user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Cria um outro usuário vinculado à empresa, mas não o user original;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetAllClientByCompanyId sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, company.CompanyId));
    }

    #region helper
    private static GetAllClientByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetAllClientByCompanyId getClientByCompanyId = new(context, checkIfUserIsLinkedCompanyUser);

        return getClientByCompanyId;
    }
    #endregion
}
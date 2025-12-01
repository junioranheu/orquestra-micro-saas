using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.ClientsFollowUps.GetAllByCompanyId;
using Orquestra.Application.UseCases.ClientsFollowUps.Shared;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ClientsFollowUps;

public sealed class GetAllClientFollowUpByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnFollowUps_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        client.CompanyId = company.CompanyId;
        context.Update(client);
        await context.SaveChangesAsync();

        ClientFollowUp follow1 = ClientFollowUpMock.Create(client, company);
        follow1.Status = true;
        await Fixture.Save(context, follow1);

        ClientFollowUp follow2 = ClientFollowUpMock.Create(client, company);
        follow2.Status = true;
        await Fixture.Save(context, follow2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientFollowUpInput input = new() { CompanyId = company.CompanyId };

        GetAllClientFollowUpByCompanyId sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(2, count);
        Assert.Equal(2, output.Count());
        Assert.Contains(output, r => r.ClientFollowUpId == follow1.ClientFollowUpId);
        Assert.Contains(output, r => r.ClientFollowUpId == follow2.ClientFollowUpId);
    }

    [Fact]
    public async Task Execute_ShouldReturnPaginatedFollowUps_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        client.CompanyId = company.CompanyId;
        context.Update(client);
        await context.SaveChangesAsync();

        List<ClientFollowUp> follows = [];

        for (int i = 0; i < 5; i++)
        {
            ClientFollowUp f = ClientFollowUpMock.Create(client, company);
            f.Status = true;
            await Fixture.Save(context, f);
            follows.Add(f);
        }

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new() { Index = 0, Limit = 3 };
        ClientFollowUpInput input = new() { CompanyId = company.CompanyId };

        GetAllClientFollowUpByCompanyId sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Equal(5, count);
        Assert.Equal(3, output.Count());
    }

    [Fact]
    public async Task Execute_ShouldReturnFilteredFollowUps_WhenClientIdMatches()
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

        Client client2 = ClientMock.Create();
        await Fixture.Save(context, client2);
        client2.CompanyId = company.CompanyId;

        context.UpdateRange(client1, client2);
        await context.SaveChangesAsync();

        ClientFollowUp f1 = ClientFollowUpMock.Create(client1, company);
        f1.Status = true;
        await Fixture.Save(context, f1);

        ClientFollowUp f2 = ClientFollowUpMock.Create(client2, company);
        f2.Status = true;
        await Fixture.Save(context, f2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientFollowUpInput input = new() { ClientId = client1.ClientId, CompanyId = company.CompanyId };

        GetAllClientFollowUpByCompanyId sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Single(output);
        Assert.Equal(1, count);
        Assert.Equal(f1.ClientFollowUpId, output.First().ClientFollowUpId);
    }

    [Fact]
    public async Task Execute_ShouldReturnFilteredFollowUps_WhenStatusMatches()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);
        client.CompanyId = company.CompanyId;

        context.Update(client);
        await context.SaveChangesAsync();

        ClientFollowUp f1 = ClientFollowUpMock.Create(client, company);
        f1.ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress;
        f1.Status = true;
        await Fixture.Save(context, f1);

        ClientFollowUp f2 = ClientFollowUpMock.Create(client, company);
        f2.ClientFollowUpStatus = ClientFollowUpStatusEnum.Completed;
        f2.Status = true;
        await Fixture.Save(context, f2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientFollowUpInput input = new() { ClientFollowUpStatus = ClientFollowUpStatusEnum.InProgress, CompanyId = company.CompanyId };

        GetAllClientFollowUpByCompanyId sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Single(output);
        Assert.Equal(1, count);
        Assert.Equal(f1.ClientFollowUpId, output.First().ClientFollowUpId);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenCompanyHasNoFollowUps()
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

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientFollowUpInput input = new();

        GetAllClientFollowUpByCompanyId sut = CreateSut(context, user);

        // Act;
        (IEnumerable<ClientFollowUpOutput> output, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.Empty(output);
        Assert.Equal(0, count);
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

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ClientFollowUpInput input = new();

        GetAllClientFollowUpByCompanyId sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(pagination, input, user.UserId, company.CompanyId));
    }

    #region helper
    private static GetAllClientFollowUpByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetAllClientFollowUpByCompanyId getClientFollowUpByCompanyId = new(context, checkIfUserIsLinkedCompanyUser);

        return getClientFollowUpByCompanyId;
    }
    #endregion
}
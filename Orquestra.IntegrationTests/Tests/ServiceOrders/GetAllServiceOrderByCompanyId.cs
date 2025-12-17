using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.GetAllByCompanyId;
using Orquestra.Application.UseCases.ServiceOrders.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.ServiceOrders;

public sealed class GetAllServiceOrderByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Outro usuário vinculado
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

        GetAllServiceOrderByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ServiceOrderInput input = new();

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(pagination, input, user.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenNoServiceOrders()
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

        GetAllServiceOrderByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ServiceOrderInput input = new();

        // Act;
        (var result, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.Empty(result);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Execute_ShouldReturnServiceOrders_WhenExists()
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


        ServiceOrder so1 = ServiceOrderMock.Create(company.CompanyId, title: "OS 1");
        await Fixture.Save(context, so1);

        ServiceOrder so2 = ServiceOrderMock.Create(company.CompanyId, title: "OS 2");
        await Fixture.Save(context, so2);

        GetAllServiceOrderByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ServiceOrderInput input = new();

        // Act;
        (IEnumerable<ServiceOrderOutput>? result, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.Equal(2, count);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Execute_ShouldFilterByTitle()
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

        ServiceOrder so1 = ServiceOrderMock.Create(company.CompanyId, title: "Instalação");
        await Fixture.Save(context, so1);

        ServiceOrder so2 = ServiceOrderMock.Create(company.CompanyId, title: "Manutenção");
        await Fixture.Save(context, so2);

        GetAllServiceOrderByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        ServiceOrderInput input = new()
        {
            Title = "insta"
        };

        // Act;
        (IEnumerable<ServiceOrderOutput>? result, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.Single(result);
        Assert.Equal(1, count);
        Assert.Equal("Instalação", result.First().Title);
    }

    [Fact]
    public async Task Execute_ShouldPaginateResults()
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

        for (int i = 0; i < 15; i++)
        {
            ServiceOrder order = ServiceOrderMock.Create(company.CompanyId, title: $"OS {i}");
            await Fixture.Save(context, order);
        }

        GetAllServiceOrderByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };
        ServiceOrderInput input = new();

        // Act;
        (IEnumerable<ServiceOrderOutput>? result, int count) = await sut.Execute(pagination, input, user.UserId, company.CompanyId);

        // Assert;
        Assert.Equal(15, count);
        Assert.Equal(10, result.Count());
    }

    #region helpers
    private static GetAllServiceOrderByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        GetAllServiceOrderByCompanyId getAllServiceOrderByCompanyId = new(context, check);

        return getAllServiceOrderByCompanyId;
    }
    #endregion
}
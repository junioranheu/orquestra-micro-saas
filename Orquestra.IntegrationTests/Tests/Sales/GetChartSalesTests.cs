using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Sales.GetChart;
using Orquestra.Application.UseCases.Sales.Shared;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Sales;

public sealed class GetChartSalesTests
{
    [Fact]
    public async Task Execute_ShouldReturnChartAndTable_WhenDataExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Premium;
        await Fixture.Save(context, company);

        Client client = ClientMock.Create();
        await Fixture.Save(context, client);

        // Inventory item (valor negativo);
        Inventory inventory = new()
        {
            InventoryId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Name = "Item estoque",
            Status = true,
            Quantity = 20,
            UnitPrice = 10,
            CreatedDate = GetDate().AddDays(-2)
        };

        await Fixture.Save(context, inventory);

        // Schedule item (valor positivo);
        Schedule schedule = new()
        {
            ScheduleId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            ClientId = client.ClientId,
            Client = client,
            Status = true,
            AmountReceived = 150,
            CreatedDate = GetDate().AddDays(-1)
        };

        await Fixture.Save(context, schedule);

        GetChartSales sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act;
        SalesOutput output = await sut.Execute(pagination, user.UserId, company.CompanyId);

        // Assert;
        Assert.NotNull(output);
        Assert.NotEmpty(output.Table);
        Assert.NotEmpty(output.Chart);

        // Inventory negativo;
        Assert.Contains(output.Table, x => x.Type == GetEnumDesc(ModuleEnum.Inventory) && x.Value < 0);

        // Scheduling positivo;
        Assert.Contains(output.Table, x => x.Type == GetEnumDesc(ModuleEnum.Scheduling) && x.Value > 0);

        Assert.Equal(2, output.TableTotalCount);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyIdIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        GetChartSales sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(pagination, user.UserId, Guid.Empty));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Nenhum vínculo criado;
        GetChartSales sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(pagination, user.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenCompanyPlanIsNotPremium()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Basic; // Não premium;
        await Fixture.Save(context, company);

        // Vincula user -> company;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetChartSales sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(pagination, user.UserId, company.CompanyId));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenNoDataExists()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        company.PlanType = PlanTypeEnum.Premium;
        await Fixture.Save(context, company);

        // Vincula user -> company;
        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        GetChartSales sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(pagination, user.UserId, company.CompanyId));
    }

    #region helpers
    private static GetChartSales CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);

        GetAllCompanyUserByCompanyId getCompanyUser = new(context);
        CheckIfUserIsLinkedCompanyUser checkLinked = new(getCompanyUser, accessor);

        GetChartSales getChartSales = new(context, checkLinked);

        return getChartSales;
    }
    #endregion
}
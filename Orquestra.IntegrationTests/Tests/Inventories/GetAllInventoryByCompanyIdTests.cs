using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.GetAllByCompanyId;
using Orquestra.Application.UseCases.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Inventories;

public sealed class GetAllInventoryByCompanyIdTests
{
    [Fact]
    public async Task Execute_ShouldReturnAllActiveInventories_ForCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        // Inventários ativos;
        Inventory inv1 = InventoryMock.Create(); 
        await Fixture.Save(context, inv1);
        inv1.CompanyId = company.CompanyId;
        inv1.Status = true;
        context.Update(inv1);
        await context.SaveChangesAsync();

        Inventory inv2 = InventoryMock.Create();
        await Fixture.Save(context, inv2);
        inv2.CompanyId = company.CompanyId;
        inv2.Status = true;
        context.Update(inv2);
        await context.SaveChangesAsync();

        // Inventário inativo;
        Inventory inv3 = InventoryMock.Create();
        await Fixture.Save(context, inv3);
        inv3.CompanyId = company.CompanyId;
        inv3.Status = false;
        context.Update(inv3);
        await context.SaveChangesAsync();

        GetAllInventoryByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act;
        (IEnumerable<Inventory> output, int count) = await sut.Execute(pagination, user.UserId, company.CompanyId);

        // Assert;
        Assert.Equal(2, count);
        Assert.Contains(output, x => x.InventoryId == inv1.InventoryId);
        Assert.Contains(output, x => x.InventoryId == inv2.InventoryId);
        Assert.DoesNotContain(output, x => x.InventoryId == inv3.InventoryId);
    }

    [Fact]
    public async Task Execute_ShouldReturnEmpty_WhenNoInventoriesExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser companyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = user.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, companyUser);

        GetAllInventoryByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act;
        (IEnumerable<Inventory> output, int count) = await sut.Execute(pagination, user.UserId, company.CompanyId);

        // Assert;
        Assert.Empty(output);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinked()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CompanyUser otherCompanyUser = new()
        {
            CompanyId = company.CompanyId,
            UserId = Guid.NewGuid(),
            CompanyUserRole = CompanyUserRoleEnum.Administrator
        };

        await Fixture.Save(context, otherCompanyUser);

        // Sem vincular usuário à empresa;
        GetAllInventoryByCompanyId sut = CreateSut(context, user);

        PaginationInput pagination = new() { Index = 0, Limit = 10 };

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(pagination, user.UserId, company.CompanyId));
    }

    #region helpers
    private static GetAllInventoryByCompanyId CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        GetAllInventoryByCompanyId getAllInventoryByCompanyId = new(context, checkIfUserIsLinkedCompanyUser);

        return getAllInventoryByCompanyId;
    }
    #endregion
}
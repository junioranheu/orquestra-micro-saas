using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.Delete;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Inventories;

public sealed class DeleteInventoryTests
{
    [Fact]
    public async Task Execute_ShouldDeleteInventory_WhenUserIsLinkedAsAdmin()
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

        Inventory inventory = InventoryMock.Create();
        await Fixture.Save(context, inventory);

        inventory.CompanyId = company.CompanyId;
        context.Update(inventory);
        await context.SaveChangesAsync();

        DeleteInventory sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, inventory.InventoryId);

        // Assert;
        Inventory? result = await context.Inventories.FirstOrDefaultAsync(x => x.InventoryId == inventory.InventoryId);
        Assert.NotNull(result);
        Assert.False(result!.Status);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenInventoryDoesNotExist()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        DeleteInventory sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedAsAdmin()
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
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        Inventory inventory = InventoryMock.Create();
        await Fixture.Save(context, inventory);

        inventory.CompanyId = company.CompanyId;
        context.Update(inventory);  
        await context.SaveChangesAsync();

        DeleteInventory sut = CreateSut(context, user);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, inventory.InventoryId));
    }

    #region helpers
    private static DeleteInventory CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        DeleteInventory deleteInventory = new(context, checkIfUserIsLinkedCompanyUser);

        return deleteInventory;
    }
    #endregion
}
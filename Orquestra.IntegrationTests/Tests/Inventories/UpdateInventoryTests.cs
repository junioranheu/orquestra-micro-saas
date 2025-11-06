using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Application.UseCases.Inventories.Update;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using System.Text;

namespace Orquestra.IntegrationTests.Tests.Inventories;

public sealed class UpdateInventoryTests
{
    [Fact]
    public async Task Execute_ShouldUpdateInventory_Successfully()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Inventory existing = InventoryMock.Create();
        existing.CompanyId = company.CompanyId;
        await Fixture.Save(context, existing);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = existing.Adapt<InventoryInput>();
        input.InventoryId = existing.InventoryId;
        input.CompanyId = company.CompanyId;
        input.Name = "Updated Name";
        input.Description = "Updated Description";
        input.Quantity = 42;
        input.UnitPrice = 99.99m;

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Inventory? updated = await context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.InventoryId == existing.InventoryId);
        Assert.NotNull(updated);
        Assert.Equal(input.Name, updated!.Name);
        Assert.Equal(input.Description, updated.Description);
        Assert.Equal(input.Quantity, updated.Quantity);
        Assert.Equal(input.UnitPrice, updated.UnitPrice);
        Assert.Null(updated.Image);
        Assert.Null(updated.ImageContentType);
    }

    [Fact]
    public async Task Execute_ShouldUpdateImage_WhenProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Inventory existing = InventoryMock.Create();
        existing.CompanyId = company.CompanyId;
        await Fixture.Save(context, existing);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = existing.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        byte[] bytes = Encoding.UTF8.GetBytes("fake-image");
        using MemoryStream stream = new(bytes);

        input.ImageFormFile = new FormFile(stream, 0, bytes.Length, "image", "image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Inventory? updated = await context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.InventoryId == existing.InventoryId);
        Assert.NotNull(updated);
        Assert.Equal(bytes, updated!.Image);
        Assert.Equal("image/png", updated.ImageContentType);
    }

    [Fact]
    public async Task Execute_ShouldRemoveImage_WhenFormFileIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        // Inventário com imagem inicial;
        Inventory existing = InventoryMock.Create();
        existing.CompanyId = company.CompanyId;
        existing.Image = Encoding.UTF8.GetBytes("old-image");
        existing.ImageContentType = "image/png";
        await Fixture.Save(context, existing);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = existing.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.ImageFormFile = null; // Remover imagem;

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Inventory? updated = await context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.InventoryId == existing.InventoryId);
        Assert.NotNull(updated);
        Assert.Null(updated!.Image);
        Assert.Null(updated.ImageContentType);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenInventoryNotFound()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = new()
        {
            InventoryId = Guid.NewGuid(),
            CompanyId = company.CompanyId,
            Name = "Any"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenNameIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Inventory existing = InventoryMock.Create();
        existing.CompanyId = company.CompanyId;
        await Fixture.Save(context, existing);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = existing.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.Name = string.Empty;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenQuantityIsNegative()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Inventory existing = InventoryMock.Create();
        existing.CompanyId = company.CompanyId;
        await Fixture.Save(context, existing);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = existing.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.Quantity = -1;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenImageIsLargerThan3MB()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        Inventory existing = InventoryMock.Create();
        existing.CompanyId = company.CompanyId;
        await Fixture.Save(context, existing);

        UpdateInventory sut = CreateSut(context, user);

        InventoryInput input = existing.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        byte[] largeImage = new byte[4 * 1024 * 1024];
        using MemoryStream stream = new(largeImage);

        input.ImageFormFile = new FormFile(stream, 0, largeImage.Length, "image", "too-big.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, input));
    }

    #region helper
    private static UpdateInventory CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        UpdateInventory updateInventory = new (context, checkIfUserIsLinkedCompanyUser);

        return updateInventory;
    }
    #endregion
}
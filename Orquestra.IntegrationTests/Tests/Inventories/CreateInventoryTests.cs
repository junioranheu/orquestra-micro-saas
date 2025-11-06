using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.Create;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using System.Text;

namespace Orquestra.IntegrationTests.Tests.Inventories;

public sealed class CreateInventoryTests
{
    [Fact]
    public async Task Execute_ShouldCreateInventory_Successfully()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CreateInventory sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Inventory? created = await context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == input.Name && x.CompanyId == company.CompanyId);

        Assert.NotNull(created);
        Assert.Equal(input.Description, created!.Description);
        Assert.Equal(input.Quantity, created.Quantity);
        Assert.Equal(input.UnitPrice, created.UnitPrice);
        Assert.Null(created.Image);
        Assert.Null(created.ImageContentType);
    }

    [Fact]
    public async Task Execute_ShouldSaveImage_WhenImageIsProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CreateInventory sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        // Simula upload de imagem;
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
        Inventory? created = await context.Inventories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == input.Name);

        Assert.NotNull(created);
        Assert.NotNull(created!.Image);
        Assert.Equal("image/png", created.ImageContentType);
        Assert.Equal(bytes, created.Image);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenNameIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CreateInventory sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
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

        CreateInventory sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.Quantity = -5;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUnitPriceIsNegative()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        CreateInventory sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.UnitPrice = -10;

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

        CreateInventory sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        // Cria imagem > 3MB;
        byte[] largeImage = new byte[4 * 1024 * 1024];
        using MemoryStream stream = new(largeImage);

        input.ImageFormFile = new FormFile(stream, 0, largeImage.Length, "image", "image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Execute(user.UserId, input));
    }

    #region helper
    private static CreateInventory CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        CreateInventory createInventory = new(context, checkIfUserIsLinkedCompanyUser);

        return createInventory;
    }
    #endregion
}
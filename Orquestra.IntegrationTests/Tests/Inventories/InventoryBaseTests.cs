using Mapster;
using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Inventories.Base;
using Orquestra.Application.UseCases.Inventories.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Inventories;

public sealed class InventoryBaseTests
{
    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InventoryBase sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        // Act & Assert;
        await sut.Validate(input, user.UserId, isCreate: true);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenNameIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InventoryBase sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.Name = " "; // Inválido;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenDescriptionIsTooLong()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InventoryBase sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.Description = new string('x', 256); // > 255;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenQuantityIsNegative()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InventoryBase sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.Quantity = -1;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenUnitPriceIsNegative()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InventoryBase sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;
        input.UnitPrice = -10;

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenImageIsLargerThan3MB()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Company company = CompanyMock.Create();
        await Fixture.Save(context, company);

        InventoryBase sut = CreateSut(context, user);

        Inventory inventory = InventoryMock.Create();
        InventoryInput input = inventory.Adapt<InventoryInput>();
        input.CompanyId = company.CompanyId;

        byte[] largeImage = new byte[4 * 1024 * 1024];
        using MemoryStream stream = new(largeImage);

        input.ImageFormFile = new FormFile(stream, 0, largeImage.Length, "image", "large.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        // Act & Assert;
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Validate(input, user.UserId, isCreate: true));
    }

    #region helper
    private static InventoryBase CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        InventoryBase inventoryBase = new(context, checkIfUserIsLinkedCompanyUser);

        return inventoryBase;
    }
    #endregion
}
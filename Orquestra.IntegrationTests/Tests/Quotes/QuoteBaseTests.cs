using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Base;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;
using static Orquestra.Utils.Fixtures.Get;

namespace Orquestra.IntegrationTests.Tests.Quotes;

public sealed class QuoteBaseTests
{
    [Fact]
    public async Task Validate_ShouldThrow_WhenTitleIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "A",
            Observation = "Obs test",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item ok",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenObservationIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento Válido",
            Observation = new string('x', 260),
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item ok",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenNoItems()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items = []
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenItemTitleInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenItemQuantityInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item",
                    Quantity = -1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenItemUnitPriceInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item",
                    Quantity = 1,
                    UnitPrice = -10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento Top",
            Observation = " tudo ok ",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item ok",
                    Quantity = 5,
                    UnitPrice = 35
                }
            ]
        };

        // Act;
        await sut.Validate(input, user.UserId);

        // Assert;
        Assert.Equal("Orçamento Top", input.Title);
    }

    [Fact]
    public async Task Validate_ShouldNotThrow_WhenValidUntilIsNull()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            ValidUntil = null,
            Items =
            [
                new QuoteItem
                {
                    Title = "Item",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await sut.Validate(input, user.UserId);
    }

    [Fact]
    public async Task Validate_ShouldThrow_WhenValidUntilBeforeToday()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            ValidUntil = GetDate().Date.AddDays(-2),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Validate(input, user.UserId));
    }

    [Fact]
    public async Task Validate_ShouldNotThrow_WhenValidUntilIsToday()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            ValidUntil = GetDate(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await sut.Validate(input, user.UserId);
    }

    [Fact]
    public async Task Validate_ShouldNotThrow_WhenValidUntilIsFuture()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteBase sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            ValidUntil = GetDate().Date.AddDays(7),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await sut.Validate(input, user.UserId);
    }

    #region helpers
    private static QuoteBase CreateSut(Context context, User user)
    {
        IHttpContextAccessor httpContextAccessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getCompanyUserByCompanyId = new(context);
        CheckIfUserIsLinkedCompanyUser checkIfUserIsLinkedCompanyUser = new(getCompanyUserByCompanyId, httpContextAccessor);

        QuoteBase quoteBase = new(checkIfUserIsLinkedCompanyUser);

        return quoteBase;
    }
    #endregion
}
using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Create;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Domain.Entities;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Quotes;

public sealed class CreateQuoteTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "A", // Inválido;
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item válido",
                    Quantity = 1,
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldCreateQuote_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateQuote sut = CreateSut(context, user);

        Guid companyId = Guid.NewGuid();

        QuoteInput input = new()
        {
            Title = "Orçamento Teste",
            Observation = "Alguma observação válida",
            CompanyId = companyId,
            Items =
            [
                new QuoteItem
                {
                    Title = "Item 1",
                    Quantity = 2,
                    UnitPrice = 50
                },
                new QuoteItem
                {
                    Title = "Item 2",
                    Quantity = 1,
                    UnitPrice = 100
                }
            ]
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Quote? quote = context.Quotes.FirstOrDefault(x => x.Title == "Orçamento Teste");
        Assert.NotNull(quote);
        Assert.Equal(companyId, quote!.CompanyId);

        List<QuoteItem> items = [.. context.QuoteItems.Where(x => x.QuoteId == quote.QuoteId)];

        Assert.Equal(2, items.Count);
        Assert.Contains(items, x => x.Title == "Item 1" && x.Quantity == 2);
        Assert.Contains(items, x => x.Title == "Item 2" && x.Quantity == 1);
    }

    [Fact]
    public async Task Execute_ShouldSaveItems_WhenProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Orçamento com Itens",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item X",
                    Quantity = 10,
                    UnitPrice = 25
                }
            ]
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Quote? quote = context.Quotes.FirstOrDefault(q => q.Title == "Orçamento com Itens");
        Assert.NotNull(quote);

        List<QuoteItem> savedItems = [.. context.QuoteItems.Where(x => x.QuoteId == quote!.QuoteId)];

        Assert.Single(savedItems);

        QuoteItem item = savedItems[0];
        Assert.Equal("Item X", item.Title);
        Assert.Equal(10, item.Quantity);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Teste",
            Observation = "obs",
            CompanyId = Guid.NewGuid(), // User NÃO está vinculado;
            Items = []
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenItemsIsEmpty()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Teste",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items = [] // Proibido;
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenAnyItemIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        CreateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            Title = "Válido",
            Observation = "obs",
            CompanyId = Guid.NewGuid(),
            Items =
            [
                new QuoteItem
                {
                    Title = "Item inválido",
                    Quantity = 0, // Inválido;
                    UnitPrice = 10
                }
            ]
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    #region helpers
    private static CreateQuote CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        return new CreateQuote(context, check);
    }
    #endregion
}
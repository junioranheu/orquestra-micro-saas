using Microsoft.AspNetCore.Http;
using Orquestra.Application.UseCases.CompanyUsers.CheckIfUserIsLinked;
using Orquestra.Application.UseCases.CompanyUsers.GetAllByCompanyId;
using Orquestra.Application.UseCases.Quotes.Shared;
using Orquestra.Application.UseCases.Quotes.Update;
using Orquestra.Domain.Consts;
using Orquestra.Domain.Entities;
using Orquestra.Domain.Enums;
using Orquestra.Infrastructure.Data;
using Orquestra.IntegrationTests.Fixtures;
using Orquestra.IntegrationTests.Fixtures.Mocks;

namespace Orquestra.IntegrationTests.Tests.Quotes;

public sealed class UpdateQuoteTests
{
    [Fact]
    public async Task Execute_ShouldThrow_WhenQuoteDoesNotExist()
    {
        // Arrange
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        UpdateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            QuoteId = Guid.NewGuid(), // Não existe;
            Title = "Teste",
            Observation = "obs",
            CompanyId = Guid.NewGuid()
        };

        // Act & Assert
        KeyNotFoundException ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.Execute(user.UserId, input));

        Assert.Equal(SystemConsts.Warnings.NotFoundData, ex.Message);
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenInputIsInvalid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Quote quote = QuoteMock.Create(user.UserId);
        await Fixture.Save(context, quote);

        UpdateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            QuoteId = quote.QuoteId,
            Title = "A", // Inválido;
            Observation = "obs",
            CompanyId = quote.CompanyId
        };

        // Act & Assert;
        await Assert.ThrowsAsync<ArgumentException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldThrow_WhenUserIsNotLinkedToCompany()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Quote quote = QuoteMock.Create(user.UserId);
        await Fixture.Save(context, quote);

        UpdateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            QuoteId = quote.QuoteId,
            Title = "Teste",
            Observation = "obs",
            CompanyId = Guid.NewGuid() // Diferente;
        };

        // Outro user vinculado, mas não o user autenticado;
        User user2 = UserMock.Create();
        await Fixture.Save(context, user2);

        CompanyUser companyUser = new()
        {
            CompanyUserId = Guid.NewGuid(),
            CompanyId = input.CompanyId.GetValueOrDefault(),
            UserId = user2.UserId,
            CompanyUserRole = CompanyUserRoleEnum.Member
        };

        await Fixture.Save(context, companyUser);

        // Act & Assert;
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.Execute(user.UserId, input));
    }

    [Fact]
    public async Task Execute_ShouldUpdateQuote_WhenInputIsValid()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        QuoteItem existingItem = new()
        {
            QuoteItemId = Guid.NewGuid(),
            QuoteId = Guid.Empty, 
            Title = "Item Original",
            Quantity = 1,
            UnitPrice = 10,
            CreatedBy = user.UserId,
            CreatedDate = DateTime.UtcNow
        };

        Quote quote = new()
        {
            QuoteId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Title = "Original",
            Observation = "Obs original",
            ValidUntil = DateTime.UtcNow.AddDays(5),
            QuoteStatus = QuoteStatusEnum.Draft,

            CreatedBy = user.UserId,
            CreatedDate = DateTime.UtcNow,
            LastModificationBy = null,
            LastModificationDate = null,

            Items = []
        };

        existingItem.QuoteId = quote.QuoteId;
        quote.Items.Add(existingItem);

        await Fixture.Save(context, quote);

        // Input alterado;
        QuoteInput input = new()
        {
            QuoteId = quote.QuoteId,
            CompanyId = quote.CompanyId,
            Title = "Isso é um teste",
            Observation = "Olá, mundo",
            ValidUntil = quote.ValidUntil,
            QuoteStatus = quote.QuoteStatus,

            Items =
            [
                new QuoteItem
                {
                    Title = existingItem.Title,
                    Quantity = existingItem.Quantity,
                    UnitPrice = existingItem.UnitPrice
                }
            ]
        };

        UpdateQuote sut = CreateSut(context, user);

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        Quote updated = context.Quotes.First(x => x.QuoteId == quote.QuoteId);

        Assert.Equal("Isso é um teste", updated.Title);
        Assert.Equal("Olá, mundo", updated.Observation);
    }

    [Fact]
    public async Task Execute_ShouldAddNewItems_WhenItemsProvided()
    {
        // Arrange;
        Context context = Fixture.CreateContext();

        User user = UserMock.Create();
        await Fixture.Save(context, user);

        Quote quote = QuoteMock.Create(user.UserId);

        // Item existente — criado manualmente;
        QuoteItem existingItem = new()
        {
            QuoteItemId = Guid.NewGuid(),
            QuoteId = quote.QuoteId,
            Title = "Item Existente",
            Quantity = 1,
            UnitPrice = 50,
            CreatedBy = user.UserId,
            CreatedDate = DateTime.UtcNow
        };

        quote.Items.Add(existingItem);

        await Fixture.Save(context, quote);

        UpdateQuote sut = CreateSut(context, user);

        QuoteInput input = new()
        {
            QuoteId = quote.QuoteId,
            Title = quote.Title,
            Observation = "obs",
            CompanyId = quote.CompanyId,
            Items =
            [
                new QuoteItem
                {
                    Title = "Novo Item",
                    Quantity = 5,
                    UnitPrice = 10,
                    QuoteId = quote.QuoteId // Opcional, mas evita problema de tracking;
                }
            ]
        };

        // Act;
        await sut.Execute(user.UserId, input);

        // Assert;
        List<QuoteItem> items = [.. context.QuoteItems.Where(x => x.QuoteId == quote.QuoteId)];

        // Quantidade;
        Assert.Equal(4, items.Count);

        // Item novo;
        Assert.Contains(items, x => x.Title == "Novo Item");

        // Item existente mantido;
        Assert.Contains(items, x => x.Title == existingItem.Title);
    }

    #region helpers
    private static UpdateQuote CreateSut(Context context, User user)
    {
        IHttpContextAccessor accessor = Fixture.CreateIHttpContextAccessor(user);
        GetAllCompanyUserByCompanyId getAll = new(context);
        CheckIfUserIsLinkedCompanyUser check = new(getAll, accessor);

        return new UpdateQuote(context, check);
    }
    #endregion
}